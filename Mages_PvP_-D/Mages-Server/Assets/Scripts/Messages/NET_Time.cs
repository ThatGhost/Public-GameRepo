using System;
using Unity.Networking.Transport;
using UnityEngine;

namespace MagesServer.DataLayer
{
        public class NET_Time : NetMessage
    {
        public delegate void OnReceived(NetMessage msg, NetworkConnection conn);
        public static event OnReceived onReceived;

        public float clientTime;

        public NET_Time(object[] objArr) : base(OpCode.Time, objArr)
        { }

        public NET_Time(ref DataStreamReader reader) : base(ref reader)
        { }

        public override bool Deserialise(ref DataStreamReader reader)
        {
            if (base.Deserialise(ref reader))
            {
                clientTime = reader.ReadFloat();
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