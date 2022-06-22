using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagesServer.GameLayer
{
    public class PlayerInterface : MonoBehaviour
    {
        private PlayerController _playerController;

        void Start()
        {
            _playerController = GetComponent<PlayerController>();
        }

        public void PlayerInput(MagesServer.DataLayer.NetMessage message)
        {
            _playerController.Move(message);
        }

        public float GetFaceAngle()
        {
            return _playerController.GetHeadAngle();
        }

        public PlayerMoveState GetMoveState() => _playerController.GetMoveState();
    }
}
