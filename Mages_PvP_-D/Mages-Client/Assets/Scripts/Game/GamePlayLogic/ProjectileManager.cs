using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagesClient.GameLayer.Magic
{
    public class ProjectileManager : MonoBehaviour
    {
        Dictionary<int, List<ProjectileData>> _projectiles = new Dictionary<int, List<ProjectileData>>();

        public ProjectileData GetProjectile(int type, GameObject prefab)
        {
            if(_projectiles.ContainsKey(type))
            {
                foreach (var item in _projectiles[type.GetHashCode()])
                {
                    if (!item.gameObject.activeSelf)
                        return item;
                }
            }

            return AddNewProjectile(type,prefab);
        }

        private ProjectileData AddNewProjectile(int type,GameObject prefab)
        {
            if (!_projectiles.ContainsKey(type))
                _projectiles[type] = new List<ProjectileData>();

            ProjectileData projectileData = new ProjectileData();
            projectileData.gameObject = Instantiate(prefab);
            projectileData.gameObject.transform.SetParent(transform);

            projectileData.renderer = projectileData.gameObject.GetComponentInChildren<MeshRenderer>();
            projectileData.meshFilter = projectileData.gameObject.GetComponentInChildren<MeshFilter>();
            projectileData.projectile = projectileData.gameObject.GetComponent<BasicProjectile>();
            projectileData.rigidbody = projectileData.gameObject.GetComponent<Rigidbody>();
            _projectiles[type].Add(projectileData);
            return projectileData;
        }
    }

    public struct ProjectileData
    {
        public GameObject gameObject;

        public MeshRenderer renderer;
        public MeshFilter meshFilter;
        public BasicProjectile projectile;
        public Rigidbody rigidbody;
    }
}