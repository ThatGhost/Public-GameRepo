using System;
using Unity.Networking.Transport;
using UnityEngine;
using MagesServer.Enums;

namespace MagesServer.DataLayer
{
    public class NET_PFHandshake : NetMessage
    {
        public string PfId;
        public delegate void OnReceived(NetMessage msg, NetworkConnection conn);
        public static event OnReceived onReceived;
        public NET_PFHandshake(object[] objArr) : base(OpCode.S_PFhandshake, objArr)
        { }

        public NET_PFHandshake(ref DataStreamReader reader) : base(ref reader)
        { gameState = GameState.Syncronisation; }

        public override bool Deserialise(ref DataStreamReader reader)
        {
            if (base.Deserialise(ref reader))
            {
                PfId = reader.ReadFixedString128().ToString();
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
