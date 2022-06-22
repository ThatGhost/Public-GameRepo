using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;
using MagesClient.Enums;

namespace MagesClient.DataLayer
{
    public class NET_Snapshot : NetMessage
    {
        public delegate void OnReceived(NetMessage msg);
        public static event OnReceived onReceived;
        public SnapshotDataHolder snapshot;

        public NET_Snapshot(object[] objArr) : base(OpCode.S_ServerDetails, objArr)
        { }

        public NET_Snapshot(ref DataStreamReader reader) : base(ref reader)
        { gameState = GameState.Syncronisation; }

        public override bool Deserialise(ref DataStreamReader reader)
        {
            if (base.Deserialise(ref reader))
            {
                byte positioncount = reader.ReadByte();

                snapshot = new SnapshotDataHolder();
                snapshot.playerData = new List<PlayerInfo>();

                for (int i = 0; i < positioncount; i++)
                {
                    PlayerInfo playerinfo = new PlayerInfo();

                    byte compressionByte = reader.ReadByte();
                    playerinfo.Id = reader.ReadByte();
                    playerinfo.state = reader.ReadByte();

                    if ((compressionByte & 1) != 0) playerinfo.x = Mathf.HalfToFloat(reader.ReadUShort());
                    else playerinfo.x = 0;
                    if ((compressionByte & 2) != 0) playerinfo.y = Mathf.HalfToFloat(reader.ReadUShort());
                    else playerinfo.y = 0;
                    if ((compressionByte & 4) != 0) playerinfo.z = Mathf.HalfToFloat(reader.ReadUShort());
                    else playerinfo.z = 0;
                    if ((compressionByte & 8) != 0) playerinfo.Rx = Mathf.HalfToFloat(reader.ReadUShort());
                    else playerinfo.Rx = 0;
                    if ((compressionByte & 16) != 0) playerinfo.Ry = Mathf.HalfToFloat(reader.ReadUShort());
                    else playerinfo.Ry = 0;

                    snapshot.playerData.Add(playerinfo);
                }
                return true;
            }
            return false;
        }

        public override void Received()
        {
            if (onReceived != null)
                onReceived.Invoke(this);
        }

        public struct SnapshotDataHolder
        {
            public List<PlayerInfo> playerData;
            //add other world stuff that changed here 
            // |
            // v
        }

        public struct PlayerInfo
        {
            public float x;
            public float y;
            public float z;
            public float Rx;
            public float Ry;
            public byte Id;
            public byte state;
            //add more player states here
            // |
            // v
        }
    }
}
