
using System.Collections;
using System.Collections.Generic;
using MagesClient.CallBackLayer;
using MagesClient.Enums;
using UnityEngine;

namespace MagesClient.GameLayer
{
    public class OwnerController : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform Head;

        public float _maxSpeedWalking = 8;
        public float _maxSpeedRunning = 14;
        public float _maxSpeedCrouching = 5;
        public float _maxSpeedJumping = 20;

        public float _speedRunning = 70_000;
        public float _speedWalking = 40_000;
        public float _speedJumping = 30_000;
        public float _speedCrounching = 20_000;

        public float _JumpForce = 40_000;

        private float _speed = 40_000;
        private float _sideSpeed = 40_000;
        private float _maxSpeed = 40_000;
        public float _CompensationPercentage = 0.15f;

        private Vector3 _prevPosition = Vector3.zero;
        private PlayerMoveState _playerMoveState;
        private float _headPitch = 0;
        private bool _Grounded = true;
        private LayerMask _levelMask;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            _levelMask = LayerMask.GetMask("Level");
        }

        public void FixedUpdate()
        {
            if (_prevPosition != Vector3.zero && _playerMoveState != PlayerMoveState.idle)
            {
                Vector3 dir = (_prevPosition - transform.position).normalized;
                dir *= _speed * Time.fixedDeltaTime * _CompensationPercentage;
                //_rb.AddForce(dir, ForceMode.Force);
                _rb.velocity += dir;
                //lerping closer idea
            }

            if (!Physics.Raycast(transform.position + new Vector3(0, .05f, 0), new Vector3(0,-1,0), .1f, _levelMask))
            {
                _Grounded = false;
            }
            else
            {
                _Grounded = true;
            }
            if (_playerMoveState == PlayerMoveState.idle && _Grounded) _rb.velocity = Vector3.zero;
        }

        public void Move(InputBits bits)
        {
            _playerMoveState = PlayerMoveState.idle;
            //rotations
            transform.Rotate(0, bits.x, 0);
            _headPitch += bits.y;
            _headPitch = Mathf.Clamp(_headPitch, -90, 90);
            Head.localRotation = Quaternion.Euler(_headPitch, 0, 0);

            if ((bits.inputBits & (ushort)InputType.Jumping) != 0 && _Grounded)
            {
                _rb.AddForce(new Vector3(0, _JumpForce, 0), ForceMode.Impulse);
                _playerMoveState = PlayerMoveState.jumping;
            }
            else if ((bits.inputBits & (ushort)InputType.Running) != 0 && _Grounded)
            {
                _speed = _speedRunning;
                _sideSpeed = _speedWalking;
                _maxSpeed = _maxSpeedRunning;
                _playerMoveState = PlayerMoveState.running;
            }
            else if ((bits.inputBits & (ushort)InputType.Crouching) != 0 && _Grounded)
            {
                _speed = _speedCrounching;
                _sideSpeed = _speedCrounching;
                _maxSpeed = _maxSpeedCrouching;
                _playerMoveState = PlayerMoveState.crouching;
            }
            else if (_Grounded)
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

            if ((bits.inputBits & (ushort)InputType.Left) != 0)
            {
                dir += -transform.right * _sideSpeed * Time.fixedDeltaTime;
            }
            if ((bits.inputBits & (ushort)InputType.Right) != 0)
            {
                dir += transform.right * _sideSpeed * Time.fixedDeltaTime;
            }
            if ((bits.inputBits & (ushort)InputType.Up) != 0)
            {
                dir += transform.forward * _speed * Time.fixedDeltaTime;
            }
            if ((bits.inputBits & (ushort)InputType.Down) != 0)
            {
                dir += -transform.forward * _sideSpeed * Time.fixedDeltaTime;

                //_rb.AddForce(-transform.forward
                //    * _speed                                        //speed scalor
                //    * Time.fixedDeltaTime, ForceMode.Force);
            }
            _rb.velocity += dir;

            //max speed
            if (_rb.velocity.magnitude > _maxSpeed)
            {
                _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _maxSpeed);
            }
        }

        public void SetPrevPosition(Vector3 position)
        {
            _prevPosition = new Vector3(position.x == 0 ? transform.position.x : position.x,
                                        position.y == 0 ? transform.position.y : position.y,
                                        position.z == 0 ? transform.position.z : position.z);
        }

        public Vector3 GetPrevPosition() => _prevPosition;

        public enum PlayerMoveState
        {
            walking,
            running,
            idle,
            crouching,
            jumping,
            aiming
        }
    }
}