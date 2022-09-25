using System;
using Unity.Networking.Transport;
using UnityEngine;
namespace MagesServer.DataLayer
{

    public class NET_TestMessage : NetMessage
    {
        public int TestValue;
        public delegate void OnReceived(NetMessage msg, NetworkConnection conn);
        public static event OnReceived onReceived;

        public NET_TestMessage(object[] objArr) : base(OpCode.TestMessage, objArr)
        { }

        public NET_TestMessage(ref DataStreamReader reader) : base(ref reader)
        { }

        public override bool Deserialise(ref DataStreamReader reader)
        {
            if (base.Deserialise(ref reader))
            {
                TestValue = reader.ReadInt();
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