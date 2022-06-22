using System;
using Unity.Networking.Transport;
using UnityEngine;
using MagesClient.Enums;

namespace MagesClient.DataLayer
{
    public class NET_Spawn : NetMessage
    {
        public int SpawnPoint;
        public char team;
        public int Id;

        public delegate void OnReceived(NetMessage msg);
        public static event OnReceived onReceived;
        /// <summary>
        /// Param -> Opcode
        /// </summary>
        /// <param name="objArr"></param>
        public NET_Spawn(object[] objArr) : base(OpCode.S_Spawn, objArr)
        { }

        public NET_Spawn(ref DataStreamReader reader) : base(ref reader)
        { gameState = GameState.Syncronisation; }

        public override bool Deserialise(ref DataStreamReader reader)
        {
            if (base.Deserialise(ref reader))
            {
                Id = reader.ReadByte();
                SpawnPoint = reader.ReadByte();
                team = (char)reader.ReadByte();
                return true;
            }
            return false;
        }

        public override void Received()
        {
            if (onReceived != null)
                onReceived.Invoke(this);
        }
    }
}
