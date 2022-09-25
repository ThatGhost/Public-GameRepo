using MagesClient.GameLayer;

using System.Collections;
using System.Collections.Generic;
using MagesClient.DataLayer;
using UnityEngine;

namespace MagesClient.CallBackLayer
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();
        [SerializeField] private OwnerInterface _ownerInterface;

        private Dictionary<int, PlayerData> _players = new Dictionary<int, PlayerData>();
        private LocalPlayerData _localPlayerData;

        private bool _spawnedAll = false;

        private void Start()
        {
            NET_Spawn.onReceived += OnSpawn;
            NET_SpawnOthers.onReceived += OnSpawnOthers;
            NET_Snapshot.onReceived += OnSnapshot;
            NET_AlignSnapshot.onReceived += OnAlign;
        }

        private void OnSpawn(NetMessage message)
        {
            NET_Spawn msg = (NET_Spawn)message;
            _localPlayerData = new LocalPlayerData();
            _localPlayerData.id = msg.Id;
            _localPlayerData.team = msg.team;

            _ownerInterface.SetPos(_spawnPoints[msg.SpawnPoint].position);
            _ownerInterface.SetTeam(msg.team);
            _ownerInterface.transform.rotation = _spawnPoints[msg.SpawnPoint].rotation;
        }

        private void OnSpawnOthers(NetMessage message)
        {
            if (_spawnedAll)
                return;
            NET_SpawnOthers msg = (NET_SpawnOthers)message;
            foreach (var item in msg.Players)
            {
                if(item.id != _localPlayerData.id)
                {
                    PlayerData playerData = new PlayerData();
                    playerData.gameObject = Instantiate(_playerPrefab);
                    playerData.playerInterface = playerData.gameObject.GetComponent<PlayerInterface>();
                    playerData.gameObject.transform.position = _spawnPoints[item.spawnpoint].position;
                    playerData.team = item.team;
                    playerData.PFId = item.PFId;
                    playerData.playerInterface.Team = item.team;

                    _players.Add(item.id, playerData);
                }
            }
            _spawnedAll = true;
            GameClient.GameState = Enums.GameState.Running;
        }

        private void OnSnapshot(NetMessage message)
        {
            if (!_spawnedAll)
                return;
            //world specific stuff gets done somewhere else
            NET_Snapshot msg = (NET_Snapshot)message;
            foreach (var playerData in msg.snapshot.playerData)
            {
                if (playerData.Id != _localPlayerData.id)
                {
                    _players[playerData.Id].playerInterface.ReceivedPosition(new Vector3(Mathf.HalfToFloat(playerData.x), Mathf.HalfToFloat(playerData.y), Mathf.HalfToFloat(playerData.z)), 
                                                                             new Vector2(Mathf.HalfToFloat(playerData.Rx), Mathf.HalfToFloat(playerData.Ry)),
                                                                             playerData.State);
                    _players[playerData.Id].playerInterface.ReceivedAbilities(playerData.Abilities);
                }
            }

            foreach(var worldData in msg.snapshot.worldData)
            {
                //update world data
            }
        }

        private void OnAlign(NetMessage message)
        {
            NET_AlignSnapshot msg = (NET_AlignSnapshot)message;
            IntVector3D pos = new IntVector3D(msg.info.x, msg.info.y, msg.info.z);
            IntVector3D vel = new IntVector3D(msg.info.DirX, msg.info.DirY, msg.info.DirZ);
            Vector2 dir = new Vector2(Mathf.HalfToFloat(msg.info.Rx), Mathf.HalfToFloat(msg.info.Ry));
            _ownerInterface.OnAlignPosition(pos, dir, vel, msg.info.tick);
            _ownerInterface.SetHPAndMana(msg.info.Hp,msg.info.Mana);
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