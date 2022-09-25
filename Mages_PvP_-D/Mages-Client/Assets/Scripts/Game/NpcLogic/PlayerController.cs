
using System.Collections;
using System.Collections.Generic;
using MagesClient.DataLayer;
using UnityEngine;

namespace MagesClient.GameLayer
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Transform _head;

        Animator _ani;

        private Vector3 _prevPosition = Vector3.zero;
        private Vector3 _nextPosition = Vector3.zero;
        private Vector2 _prevRotation = Vector2.zero;

        private float _lastSnapTime = 1;
        private float _timer;
        private Vector2 _AniVel;

        private void Awake()
        {
            _lastSnapTime = Time.fixedDeltaTime;
            _ani = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            float fraction = _timer/_lastSnapTime;

            Vector3 Currentpos = new Vector3(
                Mathf.LerpUnclamped(_prevPosition.x,_nextPosition.x,fraction),
                Mathf.LerpUnclamped(_prevPosition.y,_nextPosition.y,fraction),
                Mathf.LerpUnclamped(_prevPosition.z,_nextPosition.z,fraction)
                );
            transform.position = Currentpos;

        }

        public void OnSnapshotPosition(Vector3 position, Vector2 mouse, byte state)
        {
            _prevPosition = transform.position;
            _nextPosition = position;
            //_lastSnapTime = _timer != 0 ? _timer : Time.fixedDeltaTime;
            _timer = 0;

            //rotations
            if (mouse.x != 0f) _prevRotation.x = mouse.x; // face
            if (mouse.y != 0f) _prevRotation.y = mouse.y;

            transform.localRotation = Quaternion.Euler(0f, _prevRotation.y, 0f);
            _head.localRotation = Quaternion.Euler(_prevRotation.x, 0f, 0f);

            Vector3 diff = _nextPosition - _prevPosition;
            diff.Normalize();
            Vector3 temp = (diff.x * transform.right) - (diff.z * transform.forward);
            temp.z = -temp.z;
            temp.Normalize();

            _AniVel.x += (temp.x * 0.5f);
            _AniVel.y += (temp.z * 0.5f);

            if (Mathf.Abs(_AniVel.x) > 1)
                _AniVel.x = 1 * Mathf.Sign(_AniVel.x);
            if (Mathf.Abs(_AniVel.y) > 1)
                _AniVel.y = 1 * Mathf.Sign(_AniVel.y);

            if(_AniVel.magnitude > 0.1f)
                _AniVel += -_AniVel * 0.1f;

            if (Mathf.Abs(_AniVel.x) <= 0.1f && diff.magnitude < 0.5f)
                _AniVel.x = 0;
            if (Mathf.Abs(_AniVel.y) <= 0.1f && diff.magnitude < 0.5f)
                _AniVel.y = 0;

            //diff = (diff.x * transform.right) + (diff.z * transform.forward);
            //Debug.Log($"temp {temp}, diff {diff}, tr right {transform.right}, tr forward{transform.forward}");
            _ani.SetFloat("Hor", _AniVel.x);
            _ani.SetFloat("Ver", _AniVel.y);

            bool running = (state & 1) != 0;
            bool crouching = (state & 2) != 0;
            bool jumping = (state & 4) != 0;
            bool walking = true;

            if (crouching)
            {
                running = false;
                walking = false;
            }
            else if (running)
            {
                walking = false;
            }

            _ani.SetBool("Crouching",crouching);
            _ani.SetBool("Running",running);
            _ani.SetBool("Walking",walking);
            _ani.SetBool("Jumping",jumping);

            //Debug.Log($"vel {_AniVel}, Crouching {crouching} , running {running}, walking {walking}");

            //print($"new position {position}, prev pos: {_prevPosition}, last time {_lastSnapTime}, time {_lastSnapTime}");
        }
    }
}