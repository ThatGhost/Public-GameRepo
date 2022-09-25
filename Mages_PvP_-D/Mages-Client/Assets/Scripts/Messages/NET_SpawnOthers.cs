using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using MagesClient.Enums;

using UnityEngine;

namespace MagesClient.DataLayer
{
    public class NET_SpawnOthers : NetMessage
    {
        public delegate void OnReceived(NetMessage msg);
        public static event OnReceived onReceived;

        public List<PlayerInfo> Players = new List<PlayerInfo>();

        public NET_SpawnOthers(ref DataStreamReader reader) : base(ref reader)
        { _GameState = GameState.Syncronisation; }

        public override bool Deserialise(ref DataStreamReader reader)
        {
            if (base.Deserialise(ref reader))
            {
                int playercount = reader.ReadByte();
                for (int i = 0; i < playercount; i++)
                {
                    PlayerInfo info = new PlayerInfo();
                    info.id = reader.ReadByte();
                    info.spawnpoint = reader.ReadByte();
                    info.PFId = reader.ReadFixedString64().ToString();
                    info.team = (char)reader.ReadByte();
                    Players.Add(info);
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

        public struct PlayerInfo
        {
            public int id;
            public int spawnpoint;
            public string PFId;
            public char team;
        }
    }
}
