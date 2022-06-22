
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MagesClient.CallBackLayer
{
    public class ConfirmationHandler : MonoBehaviour
    {
        public void HandleConfirmation(OpCode type)
        {
            switch (type)
            {
                case OpCode.TestMessage: OnTestMessageCofirmation(); break;
            }
        }

        private void OnTestMessageCofirmation()
        {
            Debug.Log("Got confirmation from Test Message!");
        }
    }
}