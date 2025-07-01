using System;
using UnityEngine;
using MyMaterials.Scripts.Weapon.Behavior.Projectile.Core;

namespace MyMaterials.Scripts.Weapon.Behavior.Projectile
{
    [CreateAssetMenu(menuName = "Weapons/Behavior/Projectiles/ProjectileBehavior")]
    public class ProjectileWeaponBehavior : WeaponBehavior
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float shootInterval;
        [SerializeField] private float lifeTime;
        [SerializeField] private float initialSpeed;

        [Header("Behaviors(Drag & Drop SO Assets)")]
        [SerializeField] private InterfaceReference<SpawnPattern.ISpawnPattern> spawnPattern;
        [SerializeField] private InterfaceReference<Movement.IProjectileMovement> movement;
        [SerializeField] private InterfaceReference<Impact.IProjectileImpact> impact;

        private bool isFiring;
        private float lastTime;

        public override void OnTriggerDown(Transform muzzle, Transform target)
        {
            isFiring = true;
            SpawnProjectile(muzzle, target);
            lastTime = Time.time;
        }

        public override void OnTriggerHold(Transform muzzle, Transform target)
        {
            if (!isFiring) return;
            if (Time.time - lastTime >= shootInterval)
            {
                SpawnProjectile(muzzle, target);
                lastTime = Time.time;
            }
        }

        public override void OnTriggerUp(Transform muzzle, Transform target) => isFiring = false;

        protected virtual void SpawnProjectile(Transform muzzle, Transform target)
        {
            if (projectilePrefab == null || spawnPattern?.Current == null)
            {
                Debug.LogError("Projectile Prefab または Spawn Pattern が設定されていません！", this);
                return;
            }

            Action<GameObject> setupLogic = (projectileGO) =>
            {
                if (projectileGO.TryGetComponent<Rigidbody>(out var rb))
                {
                    if (projectileGO.TryGetComponent<BaseProjectile>(out var projectileBase))
                    {
                        Vector3 velocity = projectileGO.transform.forward * initialSpeed;
                        
                        projectileBase.SetInitialVelocity(velocity);
                        
                        if (impact?.Current != null)
                        {
                            projectileBase.Initialize(impact.Current);
                            movement?.Current?.Setup(rb, impact.Current, target);
                        }
                    }
                }
                //弾の生存期間を設定
                Destroy(projectileGO, lifeTime);
            };
            spawnPattern.Current.Spawn(muzzle, projectilePrefab, setupLogic);
        }
    }
}