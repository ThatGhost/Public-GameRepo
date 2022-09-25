using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MagesServer.GameLayer.Magic
{
    public abstract class Modifier : ScriptableObject
    {
        [Header("General")]
        public string Name;
        public string Desc;
        public int ManaCost = 0;
        public float ReloadTime = 0;

        public virtual Spell Modify(Spell spell)
        {
            spell.SpellCooldownTime += ReloadTime;
            spell.ManaCost += ManaCost;
            return spell;
        }
    }
}