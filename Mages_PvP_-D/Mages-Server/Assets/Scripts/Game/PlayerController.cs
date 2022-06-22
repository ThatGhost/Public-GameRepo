using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagesServer.DataLayer;
using MagesServer.Enums;

namespace MagesServer.GameLayer
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Transform Head;

        public float _maxSpeedWalking = 13;
        public float _maxSpeedRunning = 15;
        public float _maxSpeedCrouching = 5;
        public float _maxSpeedJumping = 20;

        public float _speedRunning = 70_000;
        public float _speedWalking = 40_000;
        public float _speedJumping = 30_000;
        public float _speedCrounching = 20_000;

        public float _JumpForce = 40_000;

        private float _speed = 40_000;
        private float _sideSpeed = 40_000;
        private float _maxSpeed = 13;
        private float _headPitch = 0;

        private LayerMask _levelMask;
        private PlayerMoveState _playerMoveState;
        private bool _Grounded = true;

        private void Start()
        {
           _levelMask  = LayerMask.GetMask("Level");
        }

        public void FixedUpdate()
        {
            RaycastHit hit;
            if(!Physics.Raycast(transform.position + new Vector3(0,.05f,0), new Vector3(0,-1,0), out hit ,.1f, _levelMask))
            {
                _Grounded = false;
                Debug.Log("not Grounded: no collider");
            }
            else
            {
                _Grounded = true;
                Debug.Log("Grounded: "+hit.collider.gameObject.name);
            }
            if(_playerMoveState == PlayerMoveState.idle && _Grounded) _rb.velocity = Vector3.zero;
        }

        public void Move(NetMessage message)
        {
            _playerMoveState = PlayerMoveState.idle;
            NET_Input msg = (NET_Input)message;
            List<ushort> input = msg.InputFlags;

            //rotations
            for (int i = 0; i < msg.DirectionX.Count; i++)
            {
                transform.Rotate(0, msg.DirectionX[i], 0);
            }
            for (int i = 0; i < msg.DirectionY.Count; i++)
            {
                _headPitch += msg.DirectionY[i];
                _headPitch = Mathf.Clamp(_headPitch, -90, 90);
                Head.localRotation = Quaternion.Euler(0, 0, -_headPitch);
            }

            //movement
            for (int i = 0; i < input.Count; i++)
            {
                if ((input[i] & (ushort)InputType.Jumping) != 0 && _Grounded)
                {
                    _rb.AddForce(new Vector3(0, _JumpForce, 0), ForceMode.Impulse);
                    _playerMoveState = PlayerMoveState.jumping;
                }
                else if ((input[i] & (ushort)InputType.Running) != 0 && _Grounded)
                {
                    _speed = _speedRunning;
                    _sideSpeed = _speedWalking;
                    _maxSpeed = _maxSpeedRunning;
                    _playerMoveState = PlayerMoveState.running;
                }
                else if((input[i] & (ushort)InputType.Crouching) != 0 && _Grounded)
                {
                    _speed = _speedCrounching;
                    _sideSpeed = _speedCrounching;
                    _maxSpeed = _maxSpeedCrouching;
                    _playerMoveState = PlayerMoveState.crouching;
                }
                else if(_Grounded)
                {
                    _speed = _speedWalking;
                    _sideSpeed = _speedWalking;
                    _maxSpeed = _maxSpeedWalking;
                    _playerMoveState = PlayerMoveState.walking;
                }
                else
                {
                    _speed = _speedJumping;
                    _sideSpeed = _speedJumping;
                    _maxSpeed = _maxSpeedJumping;
                }

                Vector3 dir = new Vector3();

                if ((input[i] & (ushort)InputType.Left) != 0)
                {
                    dir += -transform.right * _sideSpeed * Time.fixedDeltaTime;
                }
                if ((input[i] & (ushort)InputType.Right) != 0)
                {
                    dir += transform.right * _sideSpeed * Time.fixedDeltaTime;
                }
                if ((input[i] & (ushort)InputType.Up) != 0)
                {
                    dir += transform.forward * _speed * Time.fixedDeltaTime;
                }
                if ((input[i] & (ushort)InputType.Down) != 0)
                {
                    dir += -transform.forward * _sideSpeed * Time.fixedDeltaTime;

                    //_rb.AddForce(-transform.forward
                    //    * _speed                                        //speed scalor
                    //    * Time.fixedDeltaTime, ForceMode.Force);
                }
                _rb.velocity += dir;
            }

            //max speed
            if (_rb.velocity.magnitude > _maxSpeed)
            {
                _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _maxSpeed);
            }
        }

        public float GetHeadAngle() => _headPitch;
        public PlayerMoveState GetMoveState() => _playerMoveState;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + new Vector3(0, .05f, 0), transform.position + new Vector3(0,-0.05f,0));
        }
    }

    public enum PlayerMoveState
    {
        walking = 1,
        running = 2,
        idle = 3,
        crouching = 4,
        jumping = 5,
        aiming = 6,
    }
}