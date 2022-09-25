using System;
using Unity.Networking.Transport;
using UnityEngine;
using MagesServer.Enums;

namespace MagesServer.DataLayer
{
    public class NET_AlignSnapshot : NetMessage
    {
        public NET_AlignSnapshot(object[] objArr) : base(OpCode.R_Align, objArr)
        { }

        public NET_AlignSnapshot(ref DataStreamReader reader) : base(ref reader)
        { gameState = GameState.Running; }
    }
}
