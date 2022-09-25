using System;
using Unity.Networking.Transport;
using UnityEngine;
using MagesClient.Enums;

namespace MagesClient.DataLayer
{
    public class NET_ServerDetails : NetMessage
    {
        public int ServerTickRate;
        public int PlayerCount;
        public delegate void OnReceived(NetMessage msg);
        public static event OnReceived onReceived;
        /// <summary>
        /// Param -> Opcode
        /// </summary>
        /// <param name="objArr"></param>
        public NET_ServerDetails(object[] objArr) : base(OpCode.S_ServerDetails, objArr)
        { }

        public NET_ServerDetails(ref DataStreamReader reader) : base(ref reader)
        { _GameState = GameState.Syncronisation; }

        public override bool Deserialise(ref DataStreamReader reader)
        {
            if (base.Deserialise(ref reader))
            {
                ServerTickRate = reader.ReadInt();
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
