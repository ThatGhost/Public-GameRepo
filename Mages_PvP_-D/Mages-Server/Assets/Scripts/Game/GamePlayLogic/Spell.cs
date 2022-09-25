
using System;
using System.Collections.Generic;

using UnityEngine;

namespace MagesServer.GameLayer.Magic
{    public abstract class Spell : ScriptableObject
    {
        [Header("General")]
        public string SpellName;
        public string SpellDesc;
        public int ManaCost = 0;
        public float SpellCooldownTime = 0;
        [NonSerialized] public ProjectileManager _projectileManager;

        public abstract void Activate(Vector3 start, Vector3 end, GameObject owner, char team);

        public virtual void Realign()
        {
            ManaCost = Mathf.Clamp(ManaCost,1,100);
            SpellCooldownTime = Mathf.Clamp(SpellCooldownTime,0.1f,100);
        }
    }
}