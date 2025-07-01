using System;
using UnityEngine;

namespace MyMaterials.Scripts.Weapon.Behavior.Projectile.SpawnPattern
{
    [CreateAssetMenu(menuName = "Weapons/Behavior/Projectiles/Spawn Pattern/Single")]
    public class SingleSpawnPattern : ScriptableObject, ISpawnPattern
    {
        public void Spawn(Transform muzzle, GameObject projectilePrefab, Action<GameObject> onProjectileSpawned)
        {
            //todo speedをprojectileに渡す設定を実装
            var projectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);

            onProjectileSpawned?.Invoke(projectile);
        }
    }
}