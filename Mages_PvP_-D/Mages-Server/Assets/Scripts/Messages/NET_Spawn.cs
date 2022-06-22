using System;
using Unity.Networking.Transport;
using UnityEngine;
using MagesServer.Enums;

namespace MagesServer.DataLayer
{
    public class NET_Spawn : NetMessage
    {
        public OpCode Type;
        public delegate void OnReceived(NetMessage msg, NetworkConnection conn);
        public static event OnReceived onReceived;
        public NET_Spawn(object[] objArr) : base(OpCode.S_Spawn, objArr)
        { }

        public NET_Spawn(ref DataStreamReader reader) : base(ref reader)
        { gameState = GameState.Syncronisation; }

        public override bool Deserialise(ref DataStreamReader reader)
        {
            if (base.Deserialise(ref reader))
            {
                
                return true;
            }
            return false;
        }

        public override void Received(NetworkConnection c)
        {
            if (onReceived != null)
                onReceived.Invoke(this, c);
        }
    }
}
