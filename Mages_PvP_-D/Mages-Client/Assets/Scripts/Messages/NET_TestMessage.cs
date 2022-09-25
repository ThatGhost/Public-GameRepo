using System;
using UnityEngine;
using Unity.Networking.Transport;

namespace MagesClient.DataLayer
{
    public class NET_TestMessage : NetMessage
    {
        public int TestValue;
        public delegate void OnReceived(NetMessage msg);
        public static event OnReceived onReceived;

        public NET_TestMessage(object[] objArr) : base(OpCode.TestMessage, objArr)
        { }

        public NET_TestMessage(ref DataStreamReader reader) : base(ref reader)
        { }

        public override bool Deserialise(ref DataStreamReader reader)
        {
            bool b = base.Deserialise(ref reader);
            if (b)
                TestValue = reader.ReadInt();

            return b;
        }

        public override void Received()
        {
            if (onReceived != null)
                onReceived.Invoke(this);
        }
    }
}