
using System;
using System.Collections.Generic;
using MagesServer.Enums;
using Utils;
using UnityEngine;

namespace MagesServer.GameLayer.Magic
{
    public class PlayerSpellInterface : MonoBehaviour
    {
        [SerializeField] private List<SpellBehaviour> _spellBehaviours = new List<SpellBehaviour>();
        [SerializeField] private Transform _attackPointTran;
        [SerializeField] private Transform _camTransform;

        private byte _triggeredAbilities;
        private float _maxDist = 100;

        private Deque<InputBits> _inputQueue = new Deque<InputBits>();
        private PlayerInterface _interface;
        private ProjectileManager _projectileManager;

        private void Awake()
        {
            _interface = GetComponent<PlayerInterface>();
            _projectileManager = FindObjectOfType<ProjectileManager>();
            foreach (var item in _spellBehaviours)
            {
                item.spell._projectileManager = _projectileManager;
            }
        }

        public void AddInputs(List<InputBits> inputs)
        {
            foreach (InputBits input in inputs)
            {
                _inputQueue.AddToFront(input);
            }
        }

        private void FixedUpdate()
        {
            InputBits input = new InputBits();

            if (_inputQueue.Count > 0)
                input = _inputQueue.RemoveFromBack();
            Vector3 EndPosition = FindEndPos();

            _triggeredAbilities = 0;

            if ((input.inputBits & (UInt16)InputType.Ability1) != 0 && _interface.HasEnoughMana(_spellBehaviours[0].spell.ManaCost))
            {
                if(_spellBehaviours[0].OnTriggerSpell(_attackPointTran.position, EndPosition,_interface.GetTeam()))
                {
                    _triggeredAbilities |= 1;
                    ReduceMana(_spellBehaviours[0].spell.ManaCost);
                }
            }
            if ((input.inputBits & (UInt16)InputType.Ability2) != 0 && _interface.HasEnoughMana(_spellBehaviours[1].spell.ManaCost))
            {
                if (_spellBehaviours[1].OnTriggerSpell(_attackPointTran.position, EndPosition, _interface.GetTeam()))
                {
                    _triggeredAbilities |= 2;
                    ReduceMana(_spellBehaviours[1].spell.ManaCost);
                }
            }
            if ((input.inputBits & (UInt16)InputType.Ability3) != 0 && _interface.HasEnoughMana(_spellBehaviours[2].spell.ManaCost))
            {
                if (_spellBehaviours[2].OnTriggerSpell(_attackPointTran.position, EndPosition, _interface.GetTeam()))
                {
                    _triggeredAbilities |= 4;
                    ReduceMana(_spellBehaviours[2].spell.ManaCost);
                }
            }
            if ((input.inputBits & (UInt16)InputType.Ability4) != 0 && _interface.HasEnoughMana(_spellBehaviours[3].spell.ManaCost))
            {
                if (_spellBehaviours[3].OnTriggerSpell(_attackPointTran.position, EndPosition, _interface.GetTeam()))
                {
                    _triggeredAbilities |= 8;
                    ReduceMana(_spellBehaviours[3].spell.ManaCost);
                }
            }
            if ((input.inputBits & (UInt16)InputType.Ultimate) != 0 && _interface.HasEnoughMana(_spellBehaviours[4].spell.ManaCost))
            {
                if (_spellBehaviours[4].OnTriggerSpell(_attackPointTran.position, EndPosition, _interface.GetTeam()))
                {
                    _triggeredAbilities |= 16;
                    ReduceMana(_spellBehaviours[4].spell.ManaCost);
                }
            }
        }

        private Vector3 FindEndPos()
        {
            Ray ray = new Ray(_attackPointTran.position, _camTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                return hit.point;
            }
            else
            {
                return _attackPointTran.position + _camTransform.forward.normalized * _maxDist;
            }
        }
        private void ReduceMana(int mana)
        {
            _interface.AddMana(-mana);
        }

        public byte GetTriggeredAbilities() => _triggeredAbilities;
    }
}