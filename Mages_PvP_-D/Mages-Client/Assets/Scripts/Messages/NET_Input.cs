using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using MagesClient.Enums;

namespace MagesClient.DataLayer
{
    public class NET_Input : NetMessage
    {
        public delegate void OnReceived(NetMessage msg);
        public static event OnReceived onReceived;

        public List<ushort> InputFlags;
        public List<float> DirectionX;
        public List<float> DirectionY;

        /// <summary>
        /// Param -> Opcode
        /// </summary>
        /// <param name="objArr"></param>
        public NET_Input(object[] objArr) : base(OpCode.R_Input, objArr)
        { }

        public NET_Input(ref DataStreamReader reader) : base(ref reader)
        { _GameState = GameState.Running; }

        public override bool Deserialise(ref DataStreamReader reader)
        {
            return false;
        }

        public override void Received()
        {
            if (onReceived != null)
                onReceived.Invoke(this);
        }
    }
}
