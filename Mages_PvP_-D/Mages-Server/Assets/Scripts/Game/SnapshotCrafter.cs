
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using MagesServer.DataLayer;
using MagesServer.CallBackLayer;
using MagesServer.Enums;

namespace MagesServer.GameLayer
{
    public class SnapshotCrafter : MonoBehaviour
    {
        private GameServer _gameServer;
        private PlayerManager _playerManager;
        private const int SnapShotInterval = 2;
        private Snapshot prevSnapshot = new Snapshot();

        void Start()
        {
            _playerManager = GetComponent<PlayerManager>();
            _gameServer = GetComponent<GameServer>();
            _gameServer.onTick += OnTick;
        }

        private void OnTick(float time, int tick)
        {
            if (tick % SnapShotInterval == 0)
                MakeSnapshot();
        }

        private void MakeSnapshot()
        {
            if (GameServer.GameState != Enums.GameState.Running)
                return;

            //---Make snapshot---
            Snapshot newSnapshot = new Snapshot();
            newSnapshot._playerinfo = new List<PlayerInfo>();

            foreach(var data in _playerManager._playerData)
            {
                PlayerInfo playerInfo = new PlayerInfo();
                Transform playertransform = data.Value.gameObject.transform;
                playerInfo.Id = (byte)data.Key;
                playerInfo.x = Mathf.FloatToHalf(playertransform.position.x);
                playerInfo.y = Mathf.FloatToHalf(playertransform.position.y);
                playerInfo.z = Mathf.FloatToHalf(playertransform.position.z);
                playerInfo.Ry = Mathf.FloatToHalf(playertransform.eulerAngles.y);
                playerInfo.Rx = Mathf.FloatToHalf(data.Value.Interface.GetFaceAngle());

                PlayerMoveState moveState = data.Value.Interface.GetMoveState();
                playerInfo.state = (byte)moveState;

                newSnapshot._playerinfo.Add(playerInfo);
            }


            //---send to clients----
            List<object> list = new List<object>();

            //list.Add(0xFFFF); add world state changes here

            list.Add((byte)newSnapshot._playerinfo.Count);
            for (int i = 0; i < newSnapshot._playerinfo.Count; i++)
            {
                //delta compression
                byte Sending = 0;
                if(prevSnapshot._playerinfo != null)
                {
                    if (newSnapshot._playerinfo[i].x != prevSnapshot._playerinfo[i].x)
                        Sending |= 1;
                    if (newSnapshot._playerinfo[i].y != prevSnapshot._playerinfo[i].y)
                        Sending |= 2;
                    if (newSnapshot._playerinfo[i].z != prevSnapshot._playerinfo[i].z)
                        Sending |= 4;
                    if (newSnapshot._playerinfo[i].Rx != prevSnapshot._playerinfo[i].Rx)
                        Sending |= 8;
                    if (newSnapshot._playerinfo[i].Ry != prevSnapshot._playerinfo[i].Ry)
                        Sending |= 16;
                }
                else
                {
                    Sending = byte.MaxValue;
                }

                //adding it to the message array
                list.Add(Sending);
                list.Add(newSnapshot._playerinfo[i].Id);
                list.Add(newSnapshot._playerinfo[i].state);

                if ((Sending & 1) != 0)list.Add(newSnapshot._playerinfo[i].x);
                if ((Sending & 2) != 0)list.Add(newSnapshot._playerinfo[i].y);
                if ((Sending & 4) != 0)list.Add(newSnapshot._playerinfo[i].z);
                if ((Sending & 8) != 0)list.Add(newSnapshot._playerinfo[i].Rx);
                if ((Sending & 16) != 0)list.Add(newSnapshot._playerinfo[i].Ry);
            }

            _gameServer.SendToAll(new NET_Snapshot(list.ToArray()));
            prevSnapshot = newSnapshot;
        }

        private struct Snapshot
        {
            public List<PlayerInfo> _playerinfo;
            //add other world stuff that changed here 
            // |
            // v
        }

        private struct PlayerInfo
        {
            public ushort x;
            public ushort y;
            public ushort z;
            public ushort Rx;
            public ushort Ry;
            public byte Id;
            public byte state;
            //add more player states here
            // |
            // v
        }
}
}