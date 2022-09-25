using System;
using System.Collections.Generic;
using MagesClient.CallBackLayer;
using UnityEngine;

namespace MagesClient.GameLayer.Magic
{
    public class OwnerSpellInterface : MonoBehaviour
    {
        [SerializeField] private List<SpellBehaviour> _spellBehaviours = new List<SpellBehaviour>();

        private OwnerData _ownerData;
        [SerializeField] private ProjectileManager _projectileManager;

        private void Awake()
        {
            _ownerData = GetComponent<OwnerData>();

            foreach (var item in _spellBehaviours)
            {
                item.spell._projectileManager = _projectileManager;
                item.SetTeam(_ownerData.Team);
            }

            InputHandler.onInput += OnInput;
        }

        private void OnInput(InputBits input)
        {
            if ((input.inputBits & (UInt16)InputType.Ability1) != 0 && _ownerData.HasEnoughMana(_spellBehaviours[0].spell.ManaCost))
            {
                if(_spellBehaviours[0].OnTriggerSpell())
                    _ownerData.AddMana(-_spellBehaviours[0].spell.ManaCost);
            }
            if ((input.inputBits & (UInt16)InputType.Ability2) != 0 && _ownerData.HasEnoughMana(_spellBehaviours[1].spell.ManaCost))
            {
                if (_spellBehaviours[1].OnTriggerSpell())
                    _ownerData.AddMana(-_spellBehaviours[0].spell.ManaCost);
            }
            if ((input.inputBits & (UInt16)InputType.Ability3) != 0 && _ownerData.HasEnoughMana(_spellBehaviours[2].spell.ManaCost))
            {
                if (_spellBehaviours[2].OnTriggerSpell())
                    _ownerData.AddMana(-_spellBehaviours[0].spell.ManaCost);
            }
            if ((input.inputBits & (UInt16)InputType.Ability4) != 0 && _ownerData.HasEnoughMana(_spellBehaviours[3].spell.ManaCost))
            {
                if (_spellBehaviours[3].OnTriggerSpell())
                    _ownerData.AddMana(-_spellBehaviours[0].spell.ManaCost);
            }
            if ((input.inputBits & (UInt16)InputType.Ultimate) != 0 && _ownerData.HasEnoughMana(_spellBehaviours[4].spell.ManaCost))
            {
                if (_spellBehaviours[4].OnTriggerSpell())
                    _ownerData.AddMana(-_spellBehaviours[0].spell.ManaCost);
            }
        }
    }
}