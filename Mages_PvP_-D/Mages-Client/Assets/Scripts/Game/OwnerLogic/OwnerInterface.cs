
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagesClient.CallBackLayer;
using MagesClient.DataLayer;

namespace MagesClient.GameLayer
{
    public class OwnerInterface : MonoBehaviour
    {
        [SerializeField] private int _RealignDistance;
        [SerializeField] private int _RecordedInputQLenght = 200;

        private Deque<KeyValuePair<uint, InputBits>> _inputQ = new Deque<KeyValuePair<uint, InputBits>>();

        private OwnerController _controller;
        private OwnerData _data;

        private void Start()
        {
            _controller = GetComponent<OwnerController>();
            _data = GetComponent<OwnerData>();
            InputHandler.onInput += RecordInput;
        }

        private void RecordInput(InputBits input)
        {
            _inputQ.AddToFront(new KeyValuePair<uint, InputBits>(GameClient.Tick,input));

            if(_inputQ.Count > _RecordedInputQLenght)
            {
                _inputQ.RemoveFromBack();
            }
        }

        public void SetHPAndMana(int hp, int mana)
        {
            _data.HP = hp;
            _data.Mana = mana;
        }

        public void OnAlignPosition(IntVector3D position, Vector2 Dir, IntVector3D velocity,uint tick)
        {
            if((_controller.GetPosition() - position).magnitude() > _RealignDistance)
            {
                //starting conditions
                _controller.SetPositionVelocityDir(position, velocity, Dir);

                //get correct inputs
                KeyValuePair<uint, InputBits> pair = _inputQ.RemoveFromBack();
                while (pair.Key < tick)
                {
                    pair = _inputQ.RemoveFromBack();
                }

                //record inputs and advance sim
                uint prevKey = pair.Key;
                while(pair.Key < GameClient.Tick)
                {
                    while(pair.Key == prevKey)
                    {
                        _controller.RecordInput(pair.Value);
                        prevKey = pair.Key;
                        pair = _inputQ.RemoveFromBack();
                    }
                    _controller.PhysicsUpdate(pair.Key);

                    if(_inputQ.Count > 0)
                    {
                        prevKey = pair.Key;
                        pair = _inputQ.RemoveFromBack();
                    }
                }
            }
        }

        public void ReceivedState(byte b)
        {
            //_ownerController.OnStateChange(b);
        }

        public void SetPos(Vector3 pos)
        {
            _controller.SetPos(IntVector3D.Convert(pos,1000));
        }

        public void SetTeam(char team)
        {
            _data.Team = team;
        }
    }
}