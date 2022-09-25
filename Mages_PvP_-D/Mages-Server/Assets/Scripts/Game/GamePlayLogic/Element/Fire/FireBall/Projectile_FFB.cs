
using System.Collections;
using UnityEngine;

namespace MagesServer.GameLayer.Magic
{
    public class Projectile_FFB : BasicProjectile
    {
        private bool _burn;
        private bool _curve;
        private bool _explosive;
        private float _disableTime = 3;

        public void Init(bool burn, bool explosive, bool curve, float disableTime)
        {
            _burn = burn;
            _explosive = explosive;
            _curve = curve;
            _disableTime = disableTime;
        }

        private void OnEnable()
        {
            StartCoroutine(AutoDie());

        }

        protected override void OnCollision(Collision coll)
        {
            if(coll.gameObject.tag == "Player")
            {
                _playerManager.BulletDamage(coll.transform.gameObject, Attack, Team);   
            }
            gameObject.SetActive(false);
        }

        IEnumerator AutoDie()
        {
            yield return new WaitForSeconds(_disableTime);
            gameObject.SetActive(false);
        }
    }
}