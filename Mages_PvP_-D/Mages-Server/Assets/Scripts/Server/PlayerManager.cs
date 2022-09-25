using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using MagesServer.DataLayer;
using MagesServer.GameLayer;
using MagesServer.CallBackLayer;

namespace MagesServer.CallBackLayer
{
    public class PlayerManager : MonoBehaviour
    {
        public Dictionary<int, PlayerData> _playerData = new Dictionary<int, PlayerData>();
        private Dictionary<GameObject, int> _objectToId = new Dictionary<GameObject, int>();

        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();

        private GameServer _gameServer;
        private int _spawnPositionCounter = 0;

        private void Start()
        {
            _gameServer = GetComponent<GameServer>();
            _gameServer.onConnection += AddPlayer;
            _gameServer.onDisconnection += RemovePlayer;
            NET_PFHandshake.onReceived += OnReceivedPlayfabId;
            NET_Input.onReceived += OnInput;
        }

        public void AddPlayer(NetworkConnection conn)
        {
            PlayerData playerData = new PlayerData();

            playerData.connection = conn;
            playerData.gameObject = Instantiate(_playerPrefab);
            playerData.spawnPoint = _spawnPositionCounter++;
            //playerData.team = _spawnPositionCounter >= 7 ? 'R' : 'B';
            playerData.team = _spawnPositionCounter % 2 == 0 ? 'R' : 'B';

            playerData.gameObject.transform.rotation = _spawnPoints[playerData.spawnPoint].rotation;
            playerData.Interface = playerData.gameObject.GetComponent<PlayerInterface>();
            playerData.Interface.SetPos(_spawnPoints[playerData.spawnPoint].position);
            playerData.Interface.SetTeam(playerData.team);

            _playerData.Add(conn.InternalId, playerData);
            _objectToId.Add(playerData.gameObject,conn.InternalId);
            SendSpawnMessage(conn);
        }

        public void SendSpawnMessage(NetworkConnection conn)
        {
            PlayerData playerData = _playerData[conn.InternalId];
            _gameServer.SendToClientReliable(new NET_Spawn(new object[] { (byte)conn.InternalId, (byte)playerData.spawnPoint, playerData.team }), conn);
        }

        public void RemovePlayer(NetworkConnection conn)
        {
            Destroy(_playerData[conn.InternalId].gameObject);
            _playerData.Remove(conn.InternalId);
        }

        public void SendFullPlayerDataToAll()
        {
            List<object> dataArray = new List<object>();
            dataArray.Add((byte)_playerData.Count);
            foreach (var data in _playerData)
            {
                dataArray.Add((byte)data.Key);
                dataArray.Add((byte)data.Value.spawnPoint);
                dataArray.Add((string)data.Value.PFId);
                dataArray.Add((char)data.Value.team);
            }
            _gameServer.SendToAllReliable(new NET_SpawnOthers(dataArray.ToArray()));
        }

        private void OnReceivedPlayfabId(NetMessage message, NetworkConnection conn)
        {
            NET_PFHandshake msg = (NET_PFHandshake)message;
            _playerData[conn.InternalId].PFId = msg.PfId;

            //PfAPI call (get user data) to OnReceivedInfo
        }

        private void OnInput(NetMessage message, NetworkConnection conn)
        {
            if(_playerData.ContainsKey(conn.InternalId))
            {
                NET_Input msg = (NET_Input)message;
                List<InputBits> inputs = new List<InputBits>();
                for (int i = 0; i < msg.InputFlags.Count; i++)
                {
                    InputBits input = new InputBits();
                    input.x = msg.DirectionX[i];
                    input.y = msg.DirectionY[i];
                    input.inputBits = msg.InputFlags[i];
                    inputs.Add(input);
                }
                _playerData[conn.InternalId].Interface.PlayerInput(inputs);
            }
        }

        public int GetPlayerCount()
        {
            return _playerData.Count;
        }

        public void BulletDamage(GameObject g, int att, char team)
        {
            if(_objectToId.ContainsKey(g))
            {
                int id = _objectToId[g];
                if(team != _playerData[id].team)
                    _playerData[id].Interface.AddHP(-att);
            }
        }

        public class PlayerData
        {
            public GameObject gameObject;
            public PlayerInterface Interface;
            public NetworkConnection connection;
            public char team;
            public string PFId;
            public int spawnPoint;
        }
    }
}