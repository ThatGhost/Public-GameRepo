
using System;
using Unity.Networking.Transport;
using UnityEngine;

namespace MagesClient.DataLayer
{
    public class NET_Confirmation : NetMessage
    {
        public OpCode Type;
        public delegate void OnReceived(NetMessage msg);
        public static event OnReceived onReceived;

        /// <summary>
        /// Param -> Opcode
        /// </summary>
        /// <param name="objArr"></param>
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

        public override void Received()
        {
            if (onReceived != null)
                onReceived.Invoke(this);
        }
    }
}