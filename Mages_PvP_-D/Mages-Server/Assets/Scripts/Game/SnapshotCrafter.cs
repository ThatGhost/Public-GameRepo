
using System.Collections.Generic;

using UnityEngine;
using MagesServer.DataLayer;
using Utils;
using System;

namespace MagesServer.CallBackLayer
{
    public class SnapshotCrafter : MonoBehaviour
    {
        [SerializeField] private int[] _worldSnapInterVal;
        [SerializeField] private int _worldSnapCount;
        [SerializeField] private int _snapshotSendAmount = 4;
        [SerializeField] private int _snapShotAlignInterval = 40;

        private GameServer _gameServer;
        private PlayerManager _playerManager;
        private Deque<SnapInfo> _toSendInfo = new Deque<SnapInfo>();

        void Start()
        {
            _playerManager = GetComponent<PlayerManager>();
            _gameServer = GetComponent<GameServer>();
            GameServer.onTick += OnTick;
        }

        private void OnTick(uint tick)
        {
            if(_toSendInfo.Count == 0)
                FillInfo(tick);

            if (tick % _snapShotAlignInterval != 0)
                MakeSnapshot(tick);
            else
                MakeAlignSnapshots(tick);
        }

        private void MakeAlignSnapshots(uint tick)
        {
            foreach(KeyValuePair<int,PlayerManager.PlayerData> pair in _playerManager._playerData)
            {
                List<object> list = new List<object>();
                list.Add((int)tick);
                IntVector3D pos = pair.Value.Interface.GetPosition();
                IntVector3D vel = pair.Value.Interface.GetVelocity();
                list.Add(pos.x);
                list.Add(pos.y);
                list.Add(pos.z);

                list.Add(vel.x);
                list.Add(vel.y);
                list.Add(vel.z);

                list.Add((UInt16)Mathf.FloatToHalf(pair.Value.Interface.GetFaceAngle()));
                list.Add((UInt16)Mathf.FloatToHalf(pair.Value.gameObject.transform.eulerAngles.y));

                list.Add((UInt16)pair.Value.Interface.GetHP());
                list.Add((UInt16)pair.Value.Interface.GetMana());

                _gameServer.SendToClient(new NET_AlignSnapshot(list.ToArray()),pair.Value.connection);
            }
        }

        private void MakeSnapshot(uint tick)
        {
            if (GameServer.GameState != Enums.GameState.Running)
                return;

            List<object> list = new List<object>();
            list.Add((int)tick);
            for (int i = 0; i < _snapshotSendAmount && _toSendInfo.Count > 0; i++)
            {
                SnapInfo info = _toSendInfo.RemoveFromBack();
                list.Add(info.GetInfoType());

                switch (info.GetInfoType())
                {
                    case 0: //world info
                        WorldInfo wInfo = (WorldInfo)info;
                        list.Add(wInfo.Id);
                        list.Add(wInfo.Type);
                        //list.Add(wInfo.data);
                        break;
                    case 1: //player info
                        PlayerInfo pInfo = (PlayerInfo)info;
                        list.Add(pInfo.Id);
                        list.Add(pInfo.x);
                        list.Add(pInfo.y);
                        list.Add(pInfo.z);
                        list.Add(pInfo.Rx);
                        list.Add(pInfo.Ry);
                        list.Add(pInfo.Abilities);
                        list.Add(pInfo.State);
                        break;
                }
            }
            list.Add(byte.MaxValue);
            _gameServer.SendToAll(new NET_Snapshot(list.ToArray()));
        }

        private void FillInfo(uint tick)
        {
            int counter = 0;
            int WorldData = 0;

            foreach(var data in _playerManager._playerData)
            {
                foreach(int i in _worldSnapInterVal) //worldinfo in between playerinfo
                {
                    if (counter == i)
                    {
                        CreateWorldInfo();
                        WorldData++;
                        break;
                    }
                }

                CreatePlayerInfo(data);
                counter++;
            }

            while(WorldData < _worldSnapCount)
            {
                CreateWorldInfo();
                WorldData++;
            }
        }

        private void CreateWorldInfo() //!!!
        {
            WorldInfo info = new WorldInfo();

            info.Id = 5;
            info.Type = 2;
            info.data = 123;

            _toSendInfo.AddToFront(info);
        }

        private void CreatePlayerInfo(KeyValuePair<int,PlayerManager.PlayerData> data)
        {
            PlayerInfo info = new PlayerInfo();

            Vector3 pos = data.Value.gameObject.transform.position;
            info.x = Mathf.FloatToHalf(pos.x);
            info.y = Mathf.FloatToHalf(pos.y);
            info.z = Mathf.FloatToHalf(pos.z);

            info.Rx = Mathf.FloatToHalf(data.Value.Interface.GetFaceAngle());
            info.Ry = Mathf.FloatToHalf(data.Value.gameObject.transform.eulerAngles.y);
            info.Id = (byte)data.Key;
            info.Abilities = data.Value.Interface.GetTriggeredAbilities();
            info.State = data.Value.Interface.GetStates();

            _toSendInfo.AddToFront(info);

        }

        private void CreateAlginPlayerInfo(KeyValuePair<int,PlayerManager.PlayerData> data) //!!!
        {
            PlayerAlignInfo info = new PlayerAlignInfo();


            _toSendInfo.AddToFront(info);
        }

        private struct Snapshot
        {
            public uint _tick;
            public List<PlayerInfo> _playerInfo;
            public List<PlayerAlignInfo> _playerAlignInfo;
            public List<WorldInfo> _worldInfo;
        }

        private interface SnapInfo
        {
            public byte GetInfoType();
        }

        private struct WorldInfo : SnapInfo
        {
            public byte Id;
            public byte Type;
            public object data;

            byte SnapInfo.GetInfoType()
            {
                return 0;
            }
        }

        private struct PlayerInfo : SnapInfo
        {
            public ushort x;
            public ushort y;
            public ushort z;
            public ushort Rx;
            public ushort Ry;
            public byte Id;
            public byte Abilities;
            public byte State;
            //add more player states here
            // |
            // v

            byte SnapInfo.GetInfoType()
            {
                return 1;
            }
        }

        private struct PlayerAlignInfo : SnapInfo
        {
            public int x;
            public int y;
            public int z;
            public int DirX;
            public int DirY;
            public int DirZ;
            public ushort Rx;
            public ushort Ry;
            public byte Id;
            byte SnapInfo.GetInfoType()
            {
                return 2;
            }
        }
    }
}