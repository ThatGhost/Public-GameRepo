using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;
using MagesClient.Enums;

namespace MagesClient.DataLayer
{
    public class NET_AlignSnapshot : NetMessage
    {
        public delegate void OnReceived(NetMessage msg);
        public static event OnReceived onReceived;
        public PlayerAlignInfo info;

        public NET_AlignSnapshot(object[] objArr) : base(OpCode.R_Align, objArr)
        { }

        public NET_AlignSnapshot(ref DataStreamReader reader) : base(ref reader)
        { _GameState = GameState.Running; }

        public override bool Deserialise(ref DataStreamReader reader)
        {
            if (base.Deserialise(ref reader))
            {
                info = new PlayerAlignInfo();

                info.tick = (uint)reader.ReadInt();
                info.x = (int)reader.ReadInt();
                info.y = (int)reader.ReadInt();
                info.z = (int)reader.ReadInt();
                info.DirX = (int)reader.ReadInt();
                info.DirY = (int)reader.ReadInt();
                info.DirZ = (int)reader.ReadInt();
                info.Rx = (ushort)reader.ReadShort();
                info.Ry = (ushort)reader.ReadShort();

                info.Hp = reader.ReadUShort();
                info.Mana = reader.ReadUShort();
                return true;
            }
            return false;
        }

        public override void Received()
        {
            if (onReceived != null)
                onReceived.Invoke(this);
        }

        public struct PlayerAlignInfo
        {
            public uint tick;
            public int x;
            public int y;
            public int z;
            public int DirX;
            public int DirY;
            public int DirZ;
            public ushort Rx;
            public ushort Ry;

            public ushort Hp;
            public ushort Mana;
        }
    }
}
