using System;
using UnityEngine;

namespace MagesClient.GameLayer.Magic
{
    public class BasicProjectile : MonoBehaviour
    {
        [NonSerialized] public int Attack;
        [NonSerialized] public char team;
        [NonSerialized] public GameObject Owner;

        protected virtual void OnCollision(Collision coll)
        {

        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject != Owner)
                OnCollision(collision);
        }
    }
}