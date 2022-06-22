using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using MagesServer.DataLayer;
using MagesServer.GameLayer;

namespace MagesServer.CallBackLayer
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private GameObject PlayerPrefab;
        [SerializeField] private List<Transform> SpawnPoints = new List<Transform>();

        private GameServer _gameServer;
        public Dictionary<int, PlayerData> _playerData = new Dictionary<int, PlayerData>();

        private int _SpawnPositionCounter = 0;

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
            playerData.gameObject = Instantiate(PlayerPrefab);
            playerData.spawnPoint = _SpawnPositionCounter++;
            playerData.team = _SpawnPositionCounter >= 6 ? 'R' : 'B';

            playerData.gameObject.transform.position = SpawnPoints[playerData.spawnPoint].position;
            playerData.gameObject.transform.rotation = SpawnPoints[playerData.spawnPoint].rotation;
            playerData.Interface = playerData.gameObject.GetComponent<PlayerInterface>();

            _playerData.Add(conn.InternalId, playerData);
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

        private void OnReceivedInfo()
        {

        }

        public int GetPlayerCount()
        {
            return _playerData.Count;
        }

        private void OnInput(NetMessage message, NetworkConnection conn)
        {
            _playerData[conn.InternalId].Interface.PlayerInput(message);
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