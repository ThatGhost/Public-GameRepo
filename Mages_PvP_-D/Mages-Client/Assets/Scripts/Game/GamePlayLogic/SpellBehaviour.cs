
using System.Collections;
using UnityEngine;

namespace MagesClient.GameLayer.Magic
{
    public class SpellBehaviour : MonoBehaviour
    {
        public Spell spell;
        public Modifier[] Modifiers = new Modifier[3];
        [SerializeField] private Transform _attackPointTransform;

        private const float _maxDist = 100;
        private bool _ableToShoot = true;
        private char _team;

        private bool _chargable;
        private bool _munitionable;
        private int _munition;
        private int _currentMunition;

        private Animator _animator;

        private Vector3 _startPos;
        private Vector3 _endPos;

        public void SetTeam(char team) => _team = team;

        public bool OnTriggerSpell()
        {
            if(_ableToShoot)
            {
                _animator.runtimeAnimatorController = spell.Animator;

                if(spell.SpellChargeUpTime != 0)
                {
                    _animator.SetTrigger("BigShot");
                    StartCoroutine(spellWarmUp(0.75f * spell.SpellChargeUpTime));
                }
                else
                {
                    _animator.SetBool("SmallShot", true);
                }

                StartCoroutine(spellCouldown(spell.SpellCooldownTime + spell.SpellChargeUpTime));
                return true;
            }
            return false;
        }

        public void Shoot()
        {
            spell.Activate(_attackPointTransform.position, FindEndPos(), gameObject, _team);
        }

        IEnumerator spellCouldown(float time)
        {
            _ableToShoot = false;
            yield return new WaitForSeconds(time);
            _ableToShoot = true;
        }

        IEnumerator spellWarmUp(float time)
        {
            yield return new WaitForSeconds(time);
            Shoot();
        }

        public void Awake()
        {
            _animator = GetComponentInChildren<Animator>();

            spell = Instantiate(spell);
            if (Modifiers[0] != null)
                if (Modifiers[0] is InstancingModifier)
                    ModMe(Modifiers[0]);
                else
                    spell = Modifiers[0].Modify(spell);

            if (Modifiers[1] != null)
                if (Modifiers[1] is InstancingModifier)
                    ModMe(Modifiers[1]);
                else
                    spell = Modifiers[1].Modify(spell);

            if (Modifiers[2] != null)
                if (Modifiers[2] is InstancingModifier)
                    ModMe(Modifiers[2]);
                else
                    spell = Modifiers[2].Modify(spell);

            spell.Realign();
        }

        private void ModMe(Modifier modRaw)
        {
            InstancingModifier mod = (InstancingModifier)modRaw;
            _chargable = mod.Chargable;
            _munition = mod.Munition;
            _munitionable = mod.Munitionable;
        }

        private Vector3 FindEndPos()
        {
            Ray ray = new Ray(_attackPointTransform.position, _attackPointTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                return hit.point;
            }
            else
            {
                return _attackPointTransform.position + _attackPointTransform.forward.normalized * _maxDist;
            }
        }
    }
}