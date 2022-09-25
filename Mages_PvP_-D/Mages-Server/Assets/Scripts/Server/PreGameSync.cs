using System;
using System.Collections.Generic;
using UnityEngine;
using MagesServer.DataLayer;
using Unity.Networking.Transport;

namespace MagesServer.CallBackLayer
{
    public class PreGameSync : MonoBehaviour
    {
        private GameServer _gameServer;
        private PlayerManager _playerManager;
        private List<SyncTargets> _playerSyncTargets = new List<SyncTargets>();
        private int _syncRate = 40; //in ticks
        [SerializeField] private int _playerTreshold;

        private void Start()
        {
            _gameServer = GetComponent<GameServer>();
            _playerManager = GetComponent<PlayerManager>();

            GameServer.onTick += OnTick;
            _gameServer.onConnection += OnConnection;
            _gameServer.onDisconnection += OnDisconnetion;
            NET_PFHandshake.onReceived += GotPFHandshakeConfirm;
        }

        private void OnTick(uint tick)
        {
            if (tick % _syncRate == 0 && _playerManager.GetPlayerCount() >= _playerTreshold)
            {
                CheckConfirms();
                foreach (SyncTargets syncTargets in _playerSyncTargets)
                {
                    if (!syncTargets.FirstSpawn)
                    {
                        _playerManager.SendSpawnMessage(syncTargets.conn);
                    }
                    if (!syncTargets.PFHandShake && syncTargets.FirstSpawn)
                    {
                        _gameServer.SendToClient(new NET_PFHandshake(new object[] { }), syncTargets.conn);
                    }
                    if (!syncTargets.OthersSpawn && CheckAllValidity())
                    {
                        _playerManager.SendFullPlayerDataToAll();
                    }
                }
            }
        }

        private bool CheckAllValidity()
        {
            bool valid = true;
            foreach (var item in _playerSyncTargets)
            {
                if(!item.PFHandShake)
                {
                    valid = false; break;
                }
            }
            return valid;
        }

        private void OnConnection(NetworkConnection conn)
        {
            SyncTargets syncTargets = new SyncTargets();
            syncTargets.conn = conn;
            _playerSyncTargets.Add(syncTargets);
        }

        private void OnDisconnetion(NetworkConnection conn)
        {
            _playerSyncTargets.Remove(_playerSyncTargets.Find(item => item.conn == conn));
        }

        public void GotPFHandshakeConfirm(NetMessage message, NetworkConnection conn)
        {
            for (int i = 0; i < _playerSyncTargets.Count; i++)
            {
                if (_playerSyncTargets[i].conn == conn)
                    _playerSyncTargets[i].PFHandShake = true;
            }
        }

        public void GotSpawnConfirm(NetworkConnection conn)
        {
            for (int i = 0; i < _playerSyncTargets.Count; i++)
            {
                if(_playerSyncTargets[i].conn == conn)
                    _playerSyncTargets[i].FirstSpawn = true;
            }
        }

        public void GotOtherSpawnConfirm(NetworkConnection conn)
        {
            for (int i = 0; i < _playerSyncTargets.Count; i++)
            {
                if (_playerSyncTargets[i].conn == conn)
                    _playerSyncTargets[i].OthersSpawn = true;
            }
        }

        private void CheckConfirms()
        {
            bool ready = true;
            foreach(SyncTargets syncTarget in _playerSyncTargets)
            {
                if(!syncTarget.FirstSpawn   
                || !syncTarget.PFHandShake   
                || !syncTarget.OthersSpawn  )
                {
                    ready = false;
                    break;
                }
            }

            if(ready)
            {
                FinishedSyncronisation();
            }
        }

        private void FinishedSyncronisation()
        {
            GameServer.GameState = Enums.GameState.Running;
            Debug.Log("SERVER:: Finished syncronisation");

            GameServer.onTick -= OnTick;
            _gameServer.onConnection -= OnConnection;
            _gameServer.onDisconnection -= OnDisconnetion;
            enabled = false;
        }

        private class SyncTargets
        {
            public SyncTargets()
            {
                PFHandShake = false;            // Playfab
                FirstSpawn = false;             // positions/teams
                OthersSpawn = false;
                conn = default(NetworkConnection);
            }

            public NetworkConnection conn;

            public bool PFHandShake;            // Playfab
            public bool FirstSpawn;             // positions/teams
            public bool OthersSpawn;            // spawn others pos/teams
        }
    }
}