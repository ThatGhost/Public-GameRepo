using UnityEngine;

namespace MagesClient.GameLayer.Magic
{
    public abstract class Modifier : ScriptableObject
    {
        [Header("General")]
        public string Name;
        public string Desc;
        public int ManaCost = 0;
        public float ReloadTime = 0;
        public float ChargeTime = 1;

        public virtual Spell Modify(Spell spell)
        {
            spell.ManaCost += ManaCost;
            spell.SpellCooldownTime += ReloadTime;

            if (ChargeTime != 1)
                spell.SpellChargeUpTime = ChargeTime;

            return spell;
        }
    }
}