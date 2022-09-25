
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MagesServer.GameLayer.Magic
{
    [System.Serializable]
    public class Spell_FireOven : Spell
    {
        public float Size = 1;
        public Color Color = Color.red;
        public float Speed = 5;
        public int Attack = 10;
        public Mesh Mesh;

        public override void Activate(Vector3 start, Vector3 end, GameObject owner, char team)
        {
            Debug.Log("Activated Oven spell... SSSSSSSS");
        }
    }
}