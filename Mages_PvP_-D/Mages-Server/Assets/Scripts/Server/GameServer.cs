using Unity.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using MagesServer.Enums;

namespace MagesServer.DataLayer
{
    public class GameServer : MonoBehaviour
    {
        public int MaxConnections;

        public delegate void ConnectionCallback(NetworkConnection conn);
        public delegate void TickCallback(float time, int tick);

        public event TickCallback onTick;
        public event ConnectionCallback onConnection;
        public event ConnectionCallback onDisconnection;

        private NetworkDriver _driver;
        private NativeList<NetworkConnection> _connections;

        private Dictionary<int, List<NetMessage>> _messages = new Dictionary<int, List<NetMessage>>();

        private const int _serverTickRate = 30;
        private float _time = 0;
        private float _nextTickTime = 1.0f / _serverTickRate;
        private int _currentTick = 0;

        public static GameState GameState = GameState.Syncronisation;

        private void Start()
        {
            NetworkSettings settings = new NetworkSettings();
            settings.WithNetworkConfigParameters(disconnectTimeoutMS: 10000);
            _driver = NetworkDriver.Create();
            NetworkEndPoint endpoint = NetworkEndPoint.LoopbackIpv4;
            endpoint.Port = 5522;
            

            if (_driver.Bind(endpoint) != 0)
                Debug.LogError("SERVER:: error binding to port " + endpoint.Port);
            else
                _driver.Listen();

            _connections = new NativeList<NetworkConnection>(MaxConnections, Allocator.Persistent);
            Debug.Log($"SERVER:: server started (Adress: {endpoint.Address})");
        }

        private void OnDestroy()
        {
            _driver.Dispose();
            _connections.Dispose();
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
            CleanUpConnections();
            AcceptNewConnections();
            UpdateMessagePump();

            if (onTick != null)
                onTick.Invoke(_time, _currentTick);

            SendMessages();
            _currentTick++;
        }

        private void CleanUpConnections()
        {
            for (int i = 0; i < _connections.Length; i++)
            {
                if (!_connections[i].IsCreated)
                {
                    _connections.RemoveAtSwapBack(i);
                    --i;
                }
            }
        }

        private void AcceptNewConnections()
        {
            NetworkConnection c;
            while ((c = _driver.Accept()) != default(NetworkConnection))
            {
                _connections.Add(c);
                Debug.Log($"SERVER:: New connection (Id: {c.InternalId})");
                onConnection.Invoke(c);
            }
        }

        private void UpdateMessagePump()
        {
            for (int i = 0; i < _connections.Length; i++)
            {
                NetworkEvent.Type cmd;
                DataStreamReader reader;
                while ((cmd = _driver.PopEventForConnection(_connections[i], out reader)) != NetworkEvent.Type.Empty)
                {
                    if (cmd == NetworkEvent.Type.Data)
                    {
                        OnData(reader, _connections[i]);
                        //message
                    }
                    else if (cmd == NetworkEvent.Type.Disconnect)
                    {
                        Debug.Log($"SERVER:: client disconnected (Id: {_connections[i].InternalId})");
                        _connections[i] = default(NetworkConnection);
                        onDisconnection.Invoke(_connections[i]);
                    }
                }
            }
        }

        private void OnData(DataStreamReader reader, NetworkConnection conn)
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
                    case OpCode.S_PFhandshake: msg = new NET_PFHandshake(ref reader); break;
                    case OpCode.R_Input: msg = new NET_Input(ref reader); break;

                    default:
                        Debug.LogError("SERVER:: Message had no OpCode ->" + (int)opcode);
                        break;
                }
                msg.Received(conn);

                if (readByte < 0)
                {
                    SendToClient(new NET_Confirmation(new object[] { (byte)opcode }), conn);
                }
            }
        }

        private void SendMessages()
        {
            for (int i = 0; i < _connections.Length; i++)
            {
                if (_messages.ContainsKey(_connections[i].InternalId) && _connections.IsCreated)
                {
                    _driver.BeginSend(_connections[i], out DataStreamWriter writer);
                    for (int j = 0; j < _messages[_connections[i].InternalId].Count && writer.Length < 500; j++)
                    {
                        _messages[_connections[i].InternalId][j].Serialize(ref writer);
                    }
                    _driver.EndSend(writer);
                }
            }
            _messages.Clear();
        }

        public void SendToClient(NetMessage msg, NetworkConnection conn)
        {
            if (!_messages.ContainsKey(conn.InternalId))
                _messages.Add(conn.InternalId, new List<NetMessage>());

            _messages[conn.InternalId].Add(msg);
        }

        public void SendToAll(NetMessage msg)
        {
            for (int i = 0; i < _connections.Length; i++)
            {
                SendToClient(msg, _connections[i]);
            }
        }

        public void SendToClientReliable(NetMessage msg, NetworkConnection conn)
        {
            msg.ReverseCode();
            SendToClient(msg, conn);
        }

        public void SendToAllReliable(NetMessage msg)
        {
            msg.ReverseCode();
            for (int i = 0; i < _connections.Length; i++)
            {
                SendToClient(msg, _connections[i]);
            }
        }

        public float GetTime() => _time;
        public int GetTickRate() => _serverTickRate;
        public GameState GetGameState() => GameState;
        public int GetActiveConnections()
        {
            int tot = 0;
            foreach(NetworkConnection c in _connections)
            {
                if (c.IsCreated)
                    tot++;
            }
            return tot;
        }
    }
}