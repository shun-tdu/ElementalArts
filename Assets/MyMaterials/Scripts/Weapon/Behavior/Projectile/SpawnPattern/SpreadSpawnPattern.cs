using System;
using UnityEngine;

namespace MyMaterials.Scripts.Weapon.Behavior.Projectile.SpawnPattern
{
    [CreateAssetMenu(menuName = "Weapons/Behavior/Projectiles/Spawn Pattern/Spread")]
    public class SpreadSpawnPattern : ScriptableObject, ISpawnPattern
    {
        [Tooltip("発射する弾の数")] public int projectileCount = 4;
        [Tooltip("拡散する角度")] public float spreadAngle = 30f;

        public void Spawn(Transform muzzle, GameObject projectilePrefab, Action<GameObject> onProjectileSpawned)
        {
            if (projectileCount <= 1)
            {
                var projectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
                onProjectileSpawned?.Invoke(projectile);
                return;
            }
            
            //発射する角度のステップを計算
            float angleStep = spreadAngle / (projectileCount - 1);
            float startingAngle = -spreadAngle / 2f;

            for (int i = 0; i < projectileCount; i++)
            {
                float currentAngle = startingAngle + angleStep * i;
                //銃口の向きを基準に、計算した角度だけ回転
                Quaternion spreadRotation = Quaternion.AngleAxis(currentAngle, muzzle.up);
                Vector3 shootDirection = spreadRotation * muzzle.forward;
                
                var projectile = Instantiate(projectilePrefab, muzzle.position, Quaternion.LookRotation(shootDirection));
                
                onProjectileSpawned?.Invoke(projectile);
            }
        }
    }
}