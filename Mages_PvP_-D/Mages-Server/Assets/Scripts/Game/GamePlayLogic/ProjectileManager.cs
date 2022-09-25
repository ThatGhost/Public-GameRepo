using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagesServer.GameLayer.Magic
{
    public class ProjectileManager : MonoBehaviour
    {
        Dictionary<int, List<ProjectileData>> _projectiles = new Dictionary<int, List<ProjectileData>>();
        private CallBackLayer.PlayerManager _playerManager;

        private void Awake()
        {
            _playerManager = GetComponent<CallBackLayer.PlayerManager>();
        }

        public ProjectileData GetProjectile(int type, GameObject prefab)
        {
            if (_projectiles.ContainsKey(type))
            {
                foreach (var item in _projectiles[type.GetHashCode()])
                {
                    if (!item.gameObject.activeSelf)
                        return item;
                }
            }

            return AddNewProjectile(type, prefab);
        }

        private ProjectileData AddNewProjectile(int type, GameObject prefab)
        {
            if (!_projectiles.ContainsKey(type))
                _projectiles[type] = new List<ProjectileData>();

            ProjectileData projectileData = new ProjectileData();
            projectileData.gameObject = Instantiate(prefab);
            projectileData.gameObject.transform.SetParent(transform);

            projectileData.renderer = projectileData.gameObject.GetComponentInChildren<MeshRenderer>();
            projectileData.meshFilter = projectileData.gameObject.GetComponentInChildren<MeshFilter>();
            projectileData.projectile = projectileData.gameObject.GetComponent<BasicProjectile>();
            projectileData.projectile.SetPlayerManager(_playerManager);
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