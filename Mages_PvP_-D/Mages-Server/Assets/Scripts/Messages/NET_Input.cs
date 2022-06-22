using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using MagesServer.Enums;

using UnityEngine;

namespace MagesServer.DataLayer
{
    public class NET_Input : NetMessage
    {
        public delegate void OnReceived(NetMessage msg, NetworkConnection conn);
        public static event OnReceived onReceived;

        public List<ushort> InputFlags;
        public List<float >DirectionX;
        public List<float >DirectionY;

        public NET_Input(object[] objArr) : base(OpCode.R_Input, objArr)
        { }

        public NET_Input(ref DataStreamReader reader) : base(ref reader)
        { gameState = GameState.Running; }

        public override bool Deserialise(ref DataStreamReader reader)
        {
            InputFlags = new List<ushort>();
            DirectionX = new List<float>();
            DirectionY = new List<float>();

            if (base.Deserialise(ref reader))
            {
                byte inputamount = reader.ReadByte();
                for (int i = 0; i < inputamount; i++)
                {
                    byte inputFlags = reader.ReadByte();
                    if ((inputFlags & 2) != 0)
                        InputFlags.Add(reader.ReadUShort());
                    else
                        InputFlags.Add(0);

                    if ((inputFlags & 4) != 0)
                        DirectionX.Add(reader.ReadFloat());
                    else
                        DirectionX.Add(0);

                    if ((inputFlags & 8) != 0)
                        DirectionY.Add(reader.ReadFloat());
                    else
                        DirectionY.Add(0);
                }
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
