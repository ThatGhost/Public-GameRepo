using MagesClient.GameLayer;

using System.Collections;
using System.Collections.Generic;
using MagesClient.DataLayer;
using UnityEngine;

namespace MagesClient.CallBackLayer
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private GameObject PlayerPrefab;
        [SerializeField] private List<Transform> SpawnPoints = new List<Transform>();
        [SerializeField] private OwnerInterface _ownerInterface;

        private Dictionary<int, PlayerData> _players = new Dictionary<int, PlayerData>();
        private LocalPlayerData _localPlayerData;

        private bool _SpawnedAll = false;

        private void Start()
        {
            NET_Spawn.onReceived += OnSpawn;
            NET_SpawnOthers.onReceived += OnSpawnOthers;
            NET_Snapshot.onReceived += OnSnapshot;
        }

        private void OnSpawn(NetMessage message)
        {
            NET_Spawn msg = (NET_Spawn)message;
            _localPlayerData = new LocalPlayerData();
            _localPlayerData.id = msg.Id;
            _localPlayerData.team = msg.team;
            _localPlayerData.position = SpawnPoints[msg.SpawnPoint].position;
            _ownerInterface.transform.position = SpawnPoints[msg.SpawnPoint].position;
            _ownerInterface.transform.rotation = SpawnPoints[msg.SpawnPoint].rotation;
        }

        private void OnSpawnOthers(NetMessage message)
        {
            if (_SpawnedAll)
                return;
            NET_SpawnOthers msg = (NET_SpawnOthers)message;
            foreach (var item in msg.Players)
            {
                if(item.id != _localPlayerData.id)
                {
                    PlayerData playerData = new PlayerData();
                    playerData.gameObject = Instantiate(PlayerPrefab);
                    playerData.playerInterface = playerData.gameObject.GetComponent<PlayerInterface>();
                    playerData.gameObject.transform.position = SpawnPoints[item.spawnpoint].position;
                    playerData.team = item.team;
                    playerData.PFId = item.PFId;
                    _players.Add(item.id, playerData);
                }
            }
            _SpawnedAll = true;
            GameClient.GameState = Enums.GameState.Running;
        }

        private void OnSnapshot(NetMessage message)
        {
            if (!_SpawnedAll)
                return;
            //world specific stuff gets done somewhere else
            NET_Snapshot msg = (NET_Snapshot)message;
            foreach (var item in msg.snapshot.playerData)
            {
                if (item.Id != _localPlayerData.id)
                {
                    _players[item.Id].playerInterface.ReceivedPosition(new Vector3(item.x, item.y, item.z), new Vector2(item.Rx,item.Ry));
                    _players[item.Id].playerInterface.ReceivedStates(item.state);
                }
                else
                {
                    _ownerInterface.ReceveidPoistion(new Vector3(item.x, item.y, item.z));
                    _ownerInterface.ReceivedState(item.state);
                }
            }
        }

        private struct LocalPlayerData
        {
            public int id;
            public char team;
            public Vector3 position;
        }

        private struct PlayerData
        {
            public GameObject gameObject;
            public PlayerInterface playerInterface;
            public string PFId;
            public char team;
        }
    }
}