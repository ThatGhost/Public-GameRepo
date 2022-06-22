using System;
using Unity.Networking.Transport;
using UnityEngine;
using MagesServer.Enums;

namespace MagesServer.DataLayer
{
    public class NET_Confirmation : NetMessage
    {
        public OpCode Type;
        public delegate void OnReceived(NetMessage msg, NetworkConnection conn);
        public static event OnReceived onReceived;
        public NET_Confirmation(object[] objArr) : base(OpCode.Confirmation, objArr)
        { }

        public NET_Confirmation(ref DataStreamReader reader) : base(ref reader)
        { }

        public override bool Deserialise(ref DataStreamReader reader)
        {
            if (base.Deserialise(ref reader))
            {
                Type = (OpCode)reader.ReadByte();
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
