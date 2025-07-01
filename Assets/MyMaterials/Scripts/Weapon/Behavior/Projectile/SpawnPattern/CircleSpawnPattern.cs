using System;
using UnityEngine;

namespace MyMaterials.Scripts.Weapon.Behavior.Projectile.SpawnPattern
{
    [CreateAssetMenu(menuName = "Weapons/Behavior/Projectiles/Spawn Pattern/Circular")]
    public class CircleSpawnPattern : ScriptableObject, ISpawnPattern
    {
        [Tooltip("発射する弾の数")] public int projectileCount = 8;
        [SerializeField] private Vector3 spawnPointOffSet;

        public void Spawn(Transform muzzle, GameObject projectilePrefab, Action<GameObject> onProjectileSpawned)
        {
            float angleStep = 360f / projectileCount;

            for (int i = 0; i < projectileCount; i++)
            {
                float currentAngle = angleStep * i;

                Quaternion rotationAroundAxis = Quaternion.AngleAxis(currentAngle, -muzzle.forward);

                Vector3 fireDirection = rotationAroundAxis * muzzle.up;

                Quaternion projectileRotation = Quaternion.LookRotation(fireDirection, muzzle.forward);

                var projectile = Instantiate(projectilePrefab, muzzle.position + spawnPointOffSet, projectileRotation);
                
                onProjectileSpawned?.Invoke(projectile);
            }
        }
    }
}