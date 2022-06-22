
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagesClient.GameLayer
{
    public class PlayerInterface : MonoBehaviour
    {
        private PlayerController _playerController;
        void Start()
        {
            _playerController = GetComponent<PlayerController>();
        }

        public void ReceivedPosition(Vector3 position, Vector2 mouse)
        {
            _playerController.OnSnapshotPosition(position, mouse);
        }

        public void ReceivedStates(byte state)
        {
            _playerController.OnSnapshotState(state);
        }
    }
}