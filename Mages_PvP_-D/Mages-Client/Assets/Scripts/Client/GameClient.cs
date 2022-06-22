using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using MagesClient.Enums;

namespace MagesClient.DataLayer
{
    public class GameClient : MonoBehaviour
    {
        public delegate void Tick(float time, int tick);
        public event Tick onTick;

        private NetworkConnection _serverConnection;
        private NetworkDriver _driver;
        private string Ip = "127.0.0.1";
        private ushort Port = 5522;

        private int _serverTickRate = 30;
        private float _time = 0;
        private float _nextTickTime = 0;
        private int _currentTick = 0;
        private bool _connected = false;

        private List<NetMessage> _messages = new List<NetMessage>();

        public static GameState GameState = GameState.Syncronisation;

        private void Start()
        {
            _driver = NetworkDriver.Create();
            _serverConnection = default;
            if (NetworkEndPoint.TryParse(Ip, Port, out NetworkEndPoint endpoint))
            {
                _serverConnection = _driver.Connect(endpoint);
            }
            Debug.Log($"CLIENT:: client started (Adress: {endpoint.Address})");
        }

        private void OnDestroy()
        {
            _driver.Dispose();
        }

        private void Update()
        {
            _time += Time.deltaTime;
            if (_time > _nextTickTime)
            {
                OnTick();
                _nextTickTime += 1.0f / _serverTickRate;
            }
            if (_time > 254)
            {
                _time = 0;
                _nextTickTime = 1.0f / _serverTickRate;
            }
        }

        private void OnTick()
        {
            _driver.ScheduleUpdate().Complete();
            CheckAlive();
            UpdateMessagePump();

            if (onTick != null)
                onTick.Invoke(_time, _currentTick);

            if (_connected)
                SendMessages();
            _currentTick++;
        }

        private void CheckAlive()
        {
            if (!_serverConnection.IsCreated)
            {
                Debug.LogError("CLIENT:: Lost connection to server");
            }
        }

        private void UpdateMessagePump()
        {
            NetworkEvent.Type cmd;
            while ((cmd = _driver.PopEventForConnection(_serverConnection, out DataStreamReader reader)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    //message
                    OnData(reader);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log($"CLIENT:: server disconnected");
                }
                else if (cmd == NetworkEvent.Type.Connect)
                {
                    _connected = true;
                    Debug.Log($"CLIENT:: server connected");
                }
            }
        }

        private void OnData(DataStreamReader reader)
        {
            while (reader.Length > reader.GetBytesRead())
            {
                sbyte readByte = (sbyte)reader.ReadByte();
                OpCode opcode = (OpCode)Mathf.Abs(readByte);
                NetMessage msg = new NetMessage();
                switch (opcode)
                {
                    case OpCode.TestMessage: msg = new NET_TestMessage(ref reader); break;
                    case OpCode.Confirmation: msg = new NET_Confirmation(ref reader); break;
                    case OpCode.Time: msg = new NET_Time(ref reader); break;
                    case OpCode.S_ServerDetails: msg = new NET_ServerDetails(ref reader); break;
                    case OpCode.S_Spawn: msg = new NET_Spawn(ref reader); break;
                    case OpCode.S_PFhandshake: msg = new NET_PFHandshake(ref reader); break;
                    case OpCode.S_SpawnOthers: msg = new NET_SpawnOthers(ref reader); break;
                    case OpCode.R_SnapShot: msg = new NET_Snapshot(ref reader); break;

                    default:
                        Debug.LogError("CLIENT:: Message had no OpCode ->" + (int)opcode);
                        break;
                }
                msg.Received();
                if (readByte < 0)
                {
                    SendToServer(new NET_Confirmation(new object[] { (byte)opcode }));
                }
            }
        }

        private void SendMessages()
        {
            if (_messages.Count > 0)
            {
                _driver.BeginSend(_serverConnection, out DataStreamWriter writer);
                for (int i = 0; i < _messages.Count && writer.Length < 508; i++)
                {
                    _messages[i].Serialize(ref writer);
                }
                _driver.EndSend(writer);
                _messages.Clear();
            }
        }

        public void SendToServer(NetMessage msg)
        {
            _messages.Add(msg);
        }

        public void SendToServerReliable(NetMessage msg)
        {
            msg.ReverseCode();
            SendToServer(msg);
        }


        public void SetTime(float time) => _time = time;
        public float GetTime() => _time;
        public int GetTickRate() => _serverTickRate;
        public void SetTickRate(int tickrate) => _serverTickRate = tickrate;
    }
}