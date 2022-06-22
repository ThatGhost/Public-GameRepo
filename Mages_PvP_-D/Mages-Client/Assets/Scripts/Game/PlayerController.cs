
using System.Collections;
using System.Collections.Generic;
using MagesClient.DataLayer;
using UnityEngine;

namespace MagesClient.GameLayer
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Transform Head;
        private Vector3 prevPosition = Vector3.zero;
        private Vector2 prevRotation = Vector2.zero;
        private byte prevState = 0;
        private bool _isRunning = false;
        private bool _isJumping = false;
        private bool _isCrouching = false;

        private void Start()
        {
            animator.applyRootMotion = true;
        }

        private void FixedUpdate()
        {
            //float sp = (transform.position - prevPosition).magnitude;
            //animator.SetFloat("Speed",sp);

            prevPosition = transform.position;
        }

        public void OnSnapshotPosition(Vector3 position, Vector2 mouse)
        {
            Vector3 temp = new Vector3(position.x == 0 ? transform.position.x : position.x,
                                       position.y == 0 ? transform.position.y : position.y,
                                       position.z == 0 ? transform.position.z : position.z);

            transform.position = temp;

            if (mouse.x != 0) prevRotation.x = -mouse.x + 10; // face
            if (mouse.y != 0) prevRotation.y = mouse.y;

            transform.rotation = Quaternion.Euler(0, prevRotation.y, 0);
        }

        public void OnSnapshotState(byte state)
        {
            if((prevState ^ state) != 0) //state change
            {
                if (((prevState ^ state) & 1) != 0) //running bit changed
                {
                    if((prevState & 1) != 0)
                        EnterRunning();
                    else
                        ExitRunning();
                }
                if (((prevState ^ state) & 2) != 0) //jumping bit changed
                {
                    if((prevState & 2) != 0)
                        EnterJumping();
                    else
                        ExitJumping();
                }
                if (((prevState ^ state) & 4) != 0) //Crouching bit changed
                {
                    if((prevState & 4) != 0)
                        EnterCrouching();
                    else
                        ExitCrouching();
                }
            }
            prevState = state;
        }

        private void EnterRunning()
        {
            _isRunning = true;
        }
        private void ExitRunning()
        {
            _isRunning = false;
        }
        private void EnterCrouching()
        {
            _isCrouching = true;
        }
        private void ExitCrouching()
        {
            _isCrouching = false;
        }
        private void EnterJumping()
        {
            _isJumping = true;
        }
        private void ExitJumping()
        {
            _isJumping= false;
        }

        private void LateUpdate()
        {
            Head.localRotation = Quaternion.Euler(0, 0, prevRotation.x);
        }
    }
}