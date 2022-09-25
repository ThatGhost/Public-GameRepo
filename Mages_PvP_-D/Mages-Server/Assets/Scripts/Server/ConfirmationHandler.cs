using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

namespace MagesServer.CallBackLayer
{
    public class ConfirmationHandler : MonoBehaviour
    {
        private PreGameSync _preGameSync;

        private void Start()
        {
            _preGameSync = GetComponent<PreGameSync>();
        }

        public void HandleConfirmation(OpCode type, NetworkConnection c)
        {
            switch (type)
            {
                case OpCode.TestMessage: break;
                case OpCode.S_ServerDetails: print("Got sync for details") ; break;
                case OpCode.S_Spawn: _preGameSync.GotSpawnConfirm(c); break;
                case OpCode.S_SpawnOthers: _preGameSync.GotOtherSpawnConfirm(c); break;
            }
        }

    }
}