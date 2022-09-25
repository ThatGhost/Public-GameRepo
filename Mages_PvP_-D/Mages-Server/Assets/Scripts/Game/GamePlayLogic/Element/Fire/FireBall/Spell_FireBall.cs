using UnityEngine;

namespace MagesServer.GameLayer.Magic
{
    public class Spell_FireBall : Spell
    {
        [Header("Specific")]
        public float Size = 1;
        public Color Color = Color.red;
        public float Speed = 5;
        public int Attack = 10;
        public Mesh Mesh;
        public bool Explosive = false;
        public bool Burn = false;
        public bool Curve = false;

        [SerializeField] private GameObject _ProjectilePrefab;

        public override void Activate(Vector3 start, Vector3 end, GameObject owner, char team)
        {
            Vector3 dir = end - start;
            dir.Normalize();

            ProjectileData data = _projectileManager.GetProjectile(this.GetHashCode(), _ProjectilePrefab);

            data.gameObject.SetActive(true);
            data.gameObject.transform.position = start;

            data.rigidbody.velocity = Vector3.zero;
            data.rigidbody.AddForce(dir * Speed, ForceMode.Impulse);

            data.meshFilter.mesh = Mesh;
            data.gameObject.transform.localScale = new Vector3(Size, Size, Size);

            data.projectile.Attack = Attack;
            data.projectile.Owner = owner;
            data.projectile.Team = team;

            Projectile_FFB projectile = (Projectile_FFB)data.projectile;
            projectile.Init(Burn,Explosive,Curve,20);
            data.projectile = projectile;
        }

        public override void Realign()
        {
            base.Realign();

            Size = Mathf.Clamp(Size,0.1f,2);
            Speed = Mathf.Clamp(Speed,1,500);
            Attack = Mathf.Clamp(Attack,0,100);
        }
    }
}