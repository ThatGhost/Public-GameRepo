
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MagesServer.GameLayer.Magic
{
    [CreateAssetMenu(fileName = "Mod_FFB_", menuName = "Mages/Magic/Fire/Mod_FFB", order = 1)]
    public class Mod_FireBall : Modifier
    {
        [Header("Specific")]
        [SerializeField] private float _size = 0;
        [SerializeField] private Color _color = Color.black;
        [SerializeField] private float _speed = 0;
        [SerializeField] private int _attack = 0;
        [SerializeField] private Mesh _mesh;
        [SerializeField] private bool _explosive;
        [SerializeField] private bool _burn;
        [SerializeField] private bool _curve;

        public override Spell Modify(Spell RawSpell)
        {
            RawSpell = base.Modify(RawSpell);

            Spell_FireBall spell = (Spell_FireBall)RawSpell;
            spell.Attack += _attack;
            spell.Size += _size;
            spell.Speed += _speed;

            if (_color != Color.black)
                spell.Color = _color;
            if (_mesh != null)
                spell.Mesh = _mesh;

            spell.Explosive = _explosive;
            spell.Burn = _burn;
            spell.Curve = _curve; 

            return spell;
        }
    }
}