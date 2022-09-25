using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using MagesClient.Enums;

namespace MagesClient.DataLayer
{
    public class GameClient : MonoBehaviour
    {
        public delegate void OnTick(uint tick);
        public static event OnTick onTick;
        public static GameState GameState = GameState.Syncronisation;
        public static uint Tick = 0;

        private NetworkConnection _serverConnection;
        private NetworkDriver _driver;
        private string _ip = "127.0.0.1";
        private ushort _port = 5522;
        private bool _connected = false;
        private List<NetMessage> _messages = new List<NetMessage>();


        private void Start()
        {
            _driver = NetworkDriver.Create();
            _serverConnection = default;
            if (NetworkEndPoint.TryParse(_ip, _port, out NetworkEndPoint endpoint))
            {
                _serverConnection = _driver.Connect(endpoint);
            }
            Debug.Log($"CLIENT:: client started (Adress: {endpoint.Address})");
        }

        private void OnDestroy()
        {
            _driver.Dispose();
        }

        private void FixedUpdate()
        {
            DoTick();
        }

        private void DoTick()
        {
            _driver.ScheduleUpdate().Complete();
            CheckAlive();
            UpdateMessagePump();

            if (onTick != null)
                onTick.Invoke(Tick);

            if (_connected)
                SendMessages();
            Tick++;
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
                    _driver.Disconnect(_serverConnection);
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
                    case OpCode.R_Align: msg = new NET_AlignSnapshot(ref reader); break;

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

        public static float Ping = 0;
        public void SetPing(float ping) => Ping = ping;
        public void SetTick(uint tick) => Tick = tick;

#if UNITY_EDITOR

        [SerializeField] private bool debug;

        private void OnDrawGizmos()
        {
            if(debug)
            {
                GUI.color = Color.black;
                GUI.Box(new Rect(10, 10, 260, 80), "");

                GUIStyle style = new GUIStyle();
                style.fontSize = 20;

                GUI.Label(new Rect(20, 20, 150, 50), "Server tickrate", style);
                GUI.Label(new Rect(190, 20, 30, 20), "" + (1/Time.fixedDeltaTime), style);

                GUI.Label(new Rect(20, 40, 150, 50), "Tick", style);
                GUI.Label(new Rect(190, 40, 30, 20), "" + Tick, style);
            }
        }

#endif

    }
}