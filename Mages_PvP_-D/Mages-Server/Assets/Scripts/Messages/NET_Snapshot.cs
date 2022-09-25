using System;
using Unity.Networking.Transport;
using UnityEngine;
using MagesServer.Enums;

namespace MagesServer.DataLayer
{
    public class NET_Snapshot : NetMessage
    {
        public NET_Snapshot(object[] objArr) : base(OpCode.R_Snapshot, objArr)
        { }

        public NET_Snapshot(ref DataStreamReader reader) : base(ref reader)
        { gameState = GameState.Running; }
    }
}
