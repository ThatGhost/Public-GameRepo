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

        public NET_Snapshot(object[] objArr) : base(OpCode.R_SnapShot, objArr)
        { }

        public NET_Snapshot(ref DataStreamReader reader) : base(ref reader)
        { _GameState = GameState.Running; }

        public override bool Deserialise(ref DataStreamReader reader)
        {
            if (base.Deserialise(ref reader))
            {
                snapshot = new SnapshotDataHolder();
                snapshot.worldData = new List<WorldInfo>();
                snapshot.playerData = new List<PlayerInfo>();
                snapshot.playerAlignData = new List<PlayerAlignInfo>();

                snapshot.tick = (uint)reader.ReadInt();

                byte val = reader.ReadByte();

                while (val != byte.MaxValue)
                {
                    switch(val)
                    {
                        case 0:
                            WorldInfo wInfo = new WorldInfo();
                            wInfo.Id = reader.ReadByte();
                            wInfo.Type = reader.ReadByte();
                            //wInfo.Data = reader.ReadByte();
                            snapshot.worldData.Add(wInfo);
                            break;
                        case 1:
                            PlayerInfo pInfo = new PlayerInfo();
                            pInfo.Id = reader.ReadByte();
                            pInfo.x = (ushort)reader.ReadShort();
                            pInfo.y = (ushort)reader.ReadShort();
                            pInfo.z = (ushort)reader.ReadShort();
                            pInfo.Rx = (ushort)reader.ReadShort();
                            pInfo.Ry = (ushort)reader.ReadShort();
                            pInfo.Abilities = reader.ReadByte();
                            pInfo.State = reader.ReadByte();
                            snapshot.playerData.Add(pInfo);
                            break;
                        case 2:
                            PlayerAlignInfo pAInfo = new PlayerAlignInfo();
                            pAInfo.Id = reader.ReadByte();
                            pAInfo.x =reader.ReadInt();
                            pAInfo.y =reader.ReadInt();
                            pAInfo.z =reader.ReadInt();
                            pAInfo.DirX = reader.ReadInt();
                            pAInfo.DirY = reader.ReadInt();
                            pAInfo.DirZ = reader.ReadInt();
                            pAInfo.Rx = (ushort)reader.ReadShort();
                            pAInfo.Ry = (ushort)reader.ReadShort();
                            snapshot.playerAlignData.Add(pAInfo);
                            break;
                        default:
                            Debug.LogError("Unknown type of info "+ val);
                            break;
                    }


                    val = reader.ReadByte();
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
            public uint tick;
            public List<PlayerInfo> playerData;
            public List<PlayerAlignInfo> playerAlignData;
            public List<WorldInfo> worldData;
        }
        public struct WorldInfo
        {
            public byte Id;
            public byte Type;
            public object data;
        }

        public struct PlayerInfo
        {
            public ushort x;
            public ushort y;
            public ushort z;
            public ushort Rx;
            public ushort Ry;
            public byte Id;
            public byte Abilities;
            public byte State;
        }

        public struct PlayerAlignInfo
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
        }
    }
}
