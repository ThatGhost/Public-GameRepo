
using System;

using Unity.Networking.Transport;

using UnityEngine;

namespace MagesClient.DataLayer
{
    public class NET_Time : NetMessage
    {
        public delegate void OnReceived(NetMessage msg);
        public static event OnReceived onReceived;

        public uint clientTick;
        public uint serverTick;
        /// <summary>
        /// Param -> Opcode
        /// </summary>
        /// <param name="objArr"></param>
        public NET_Time(object[] objArr) : base(OpCode.Time, objArr)
        { }

        public NET_Time(ref DataStreamReader reader) : base(ref reader)
        { }

        public override bool Deserialise(ref DataStreamReader reader)
        {
            if (base.Deserialise(ref reader))
            {
                clientTick = (uint)reader.ReadInt();
                serverTick = (uint)reader.ReadInt();
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