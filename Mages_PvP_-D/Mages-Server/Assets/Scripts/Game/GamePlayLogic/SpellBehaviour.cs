
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MagesServer.GameLayer.Magic
{
    public class SpellBehaviour : MonoBehaviour
    {
        public Spell spell;
        public Modifier[] Modifiers = new Modifier[3];

        private bool _ableToShoot = true;


        public bool OnTriggerSpell(Vector3 startPos, Vector3 endPos, char team)
        {
            if (_ableToShoot)
            {
                spell.Activate(startPos, endPos,gameObject,team);
                StartCoroutine(spellCooldown(spell.SpellCooldownTime));
                return true;
            }
            return false;
        }

        IEnumerator spellCooldown(float time)
        {
            _ableToShoot = false;
            yield return new WaitForSeconds(time);
            _ableToShoot = true;
        }

        public void Awake()
        {
            spell = Instantiate(spell);
            if (Modifiers[0] != null)
                spell = Modifiers[0].Modify(spell);
            if (Modifiers[1] != null)
                spell = Modifiers[1].Modify(spell);
            if (Modifiers[2] != null)
                spell = Modifiers[2].Modify(spell);

            spell.Realign();
        }
    }
}