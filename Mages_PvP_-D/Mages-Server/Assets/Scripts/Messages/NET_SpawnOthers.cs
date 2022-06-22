using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using MagesServer.Enums;
using UnityEngine;

namespace MagesServer.DataLayer
{
    public class NET_SpawnOthers : NetMessage
    {
        public delegate void OnReceived(NetMessage msg, NetworkConnection conn);
        public static event OnReceived onReceived;

        public NET_SpawnOthers(object[] objArr) : base(OpCode.S_SpawnOthers, objArr)
        { }

        public NET_SpawnOthers(ref DataStreamReader reader) : base(ref reader)
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
