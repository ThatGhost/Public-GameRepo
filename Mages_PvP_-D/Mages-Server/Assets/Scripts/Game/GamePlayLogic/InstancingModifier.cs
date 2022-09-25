using UnityEngine;

namespace MagesServer.GameLayer.Magic
{
    [CreateAssetMenu(fileName ="Mod_",menuName = "Mages/Magic/Mod_Instancing")]
    public class InstancingModifier : Modifier
    {
        [Header("Specific")]
        public bool Burst;
        public int Amount;
        public bool Chargable;
        public int Charges;
    }
}