
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MagesClient.GameLayer.Magic
{
    public class PlayerSpellController : MonoBehaviour
    {
        [SerializeField] private List<SpellBehaviour> _spellBehaviours = new List<SpellBehaviour>();

        private float _maxDist = 100;

        private void Awake()
        {
            ProjectileManager manager = FindObjectOfType<ProjectileManager>();
            foreach (var item in _spellBehaviours)
            {
                item.spell._projectileManager = manager;
            }
        }

        public void SetAllTeams(char team)
        {
            foreach (var item in _spellBehaviours)
            {
                item.SetTeam(team);
            }
        }

        public void TriggerAbilities(byte abilities)
        {
            if ((abilities & 1) != 0)
            {
                _spellBehaviours[0]?.OnTriggerSpell();
            }
            if ((abilities & 2) != 0)
            {
                _spellBehaviours[1]?.OnTriggerSpell();
            }
            if ((abilities & 4) != 0)
            {
                _spellBehaviours[2]?.OnTriggerSpell();
            }
            if ((abilities & 8) != 0)
            {
                _spellBehaviours[3]?.OnTriggerSpell();
            }
            if ((abilities & 16) != 0)
            {
                _spellBehaviours[4]?.OnTriggerSpell();
            }
        }
    }
}