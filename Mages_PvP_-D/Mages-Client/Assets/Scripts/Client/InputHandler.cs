
using System.Collections;
using System.Collections.Generic;
using MagesClient.DataLayer;
using MagesClient.GameLayer;
using MagesClient.Enums;
using UnityEngine;
using System;

namespace MagesClient.CallBackLayer
{
    public class InputHandler : MonoBehaviour
    {
        private GameClient _gameClient;
        private Queue<InputBits> _inputs = new Queue<InputBits>();

        private bool _jumpUp = false;
        private bool _jumpDown = false;

        public delegate void OnInput(InputBits bits);
        public static event OnInput onInput;

        private void Start()
        {
            _gameClient = GetComponent<GameClient>();
            GameClient.onTick += OnTick;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
                _jumpDown = true;
            if(Input.GetKeyUp(KeyCode.Space))
                _jumpUp = true;
        }

        private void FixedUpdate()
        {
            if (GameClient.GameState != GameState.Running)
                return;

            InputBits input = new InputBits();

            if (Input.GetMouseButton(0))
                input.inputBits |= (UInt16)InputType.Ability1;
            if (Input.GetKey(KeyCode.E))
                input.inputBits |= (UInt16)InputType.Ability2;
            if (Input.GetKey(KeyCode.V))
                input.inputBits |= (UInt16)InputType.Ability3;
            if (Input.GetKey(KeyCode.C))
                input.inputBits |= (UInt16)InputType.Ability4;
            if (Input.GetKey(KeyCode.Q))
                input.inputBits |= (UInt16)InputType.Ultimate;

            if (Input.GetKey(KeyCode.W))
                input.inputBits |= (UInt16)InputType.Up;
            if (Input.GetKey(KeyCode.A))
                input.inputBits |= (UInt16)InputType.Left;
            if (Input.GetKey(KeyCode.S))
                input.inputBits |= (UInt16)InputType.Down;
            if (Input.GetKey(KeyCode.D))
                input.inputBits |= (UInt16)InputType.Right;

            if(Input.GetKey(KeyCode.LeftShift))
                input.inputBits |= (UInt16)InputType.Running;
            if (Input.GetKey(KeyCode.LeftControl))
                input.inputBits |= (UInt16)InputType.Crouching;
            if (Input.GetKey(KeyCode.E))
                input.inputBits |= (UInt16)InputType.Interact;

            if (_jumpUp)
            {
                input.inputBits |= (UInt16)InputType.JumpUp;
                _jumpUp = false;
            }

            if (_jumpDown)
            {
                input.inputBits |= (UInt16)InputType.JumpDown;
                _jumpDown = false;
            }

            input.x = Input.GetAxis("Mouse X");
            input.y = Input.GetAxis("Mouse Y");
            
            _inputs.Enqueue(input);
            onInput.Invoke(input);
        }

        private void OnTick(uint tick)
        {
            List<object> inputArray = new List<object>();

            byte count = (byte)_inputs.Count;
            inputArray.Add(count);
            for (int i = 0; i < count; i++)
            {
                InputBits input = _inputs.Dequeue();
                byte inputFlags = 0;
                if (input.inputBits != 0)
                    inputFlags |= 2;
                if (input.x != 0)
                    inputFlags |= 4;
                if (input.y != 0)
                    inputFlags |= 8;

                inputArray.Add(inputFlags);

                if ((inputFlags & 2) != 0)
                    inputArray.Add((UInt16)input.inputBits);
                if ((inputFlags & 4) != 0)
                    inputArray.Add((double)input.x);
                if ((inputFlags & 8) != 0)
                    inputArray.Add((double)input.y);
            }

            if(inputArray.Count > 1)
                _gameClient.SendToServer(new NET_Input(inputArray.ToArray()));
        }

    }
    public struct InputBits
    {
        public float x;
        public float y;
        public uint inputBits;
    }
}