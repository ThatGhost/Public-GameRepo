using UnityEngine;

namespace MagesClient.GameLayer.Magic
{
    [CreateAssetMenu(fileName = "Mod_", menuName = "Mages/Magic/Mod_Instancing")]
    public class InstancingModifier : Modifier
    {
        [Header("Specific")]
        public int Amount;
        public bool Chargable;
        public int Munition;
        public bool Munitionable;
    }
}
