using System;
using Unity.Networking.Transport;
using UnityEngine;
using MagesClient.Enums;

namespace MagesClient.DataLayer
{
    public class NET_PFHandshake : NetMessage
    {
        public delegate void OnReceived(NetMessage msg);
        public static event OnReceived onReceived;
        /// <summary>
        /// Param -> Opcode
        /// </summary>
        /// <param name="objArr"></param>
        public NET_PFHandshake(object[] objArr) : base(OpCode.S_PFhandshake, objArr)
        { }

        public NET_PFHandshake(ref DataStreamReader reader) : base(ref reader)
        { gameState = GameState.Syncronisation; }

        public override bool Deserialise(ref DataStreamReader reader)
        {
            if (base.Deserialise(ref reader))
            {
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
