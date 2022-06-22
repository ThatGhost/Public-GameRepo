
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
        private Queue<InputBits> inputs = new Queue<InputBits>();
        [SerializeField] private OwnerController _ownerController;
        public float SensitivityX = 25;
        public float SensitivityY = 25;

        private void Start()
        {
            _gameClient = GetComponent<GameClient>();
            _gameClient.onTick += OnTick;
        }

        public void FixedUpdate()
        {
            if (GameClient.GameState != GameState.Running)
                return;

            InputBits input = new InputBits();

            if (Input.GetKey(KeyCode.Q))
                input.inputBits |= (UInt16)InputType.Ability1;
            if (Input.GetKey(KeyCode.X))
                input.inputBits |= (UInt16)InputType.Ability2;
            if (Input.GetKey(KeyCode.V))
                input.inputBits |= (UInt16)InputType.Ability3;
            if (Input.GetKey(KeyCode.C))
                input.inputBits |= (UInt16)InputType.Ability4;
            if (Input.GetKey(KeyCode.E))
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
            if(Input.GetKey(KeyCode.Space))
                input.inputBits |= (UInt16)InputType.Jumping;
            if(Input.GetKey(KeyCode.LeftControl))
                input.inputBits |= (UInt16)InputType.Crouching;

            input.x = Input.GetAxis("Mouse X") * Time.fixedDeltaTime * SensitivityX;
            input.y = -Input.GetAxis("Mouse Y") * Time.fixedDeltaTime * SensitivityY;
            
            if(input.inputBits != 0 || input.x != 0 || input.y != 0)
            {
                inputs.Enqueue(input);
                _ownerController.Move(input);
            }
        }

        private void OnTick(float time, int tick)
        {
            List<object> inputArray = new List<object>();

            byte count = (byte)inputs.Count;
            inputArray.Add(count);
            for (int i = 0; i < count; i++)
            {
                InputBits input = inputs.Dequeue();
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
        public UInt16 inputBits;
    }
}