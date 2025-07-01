using UnityEngine;

namespace MyMaterials.Scripts.Weapon.Behavior.Projectile.Impact
{
    [CreateAssetMenu(menuName = "Weapons/Behavior/Projectiles/Impact/SingleHit")]
    public class SingleHitImpact : ScriptableObject, IProjectileImpact
    {
        public float damage;

        public void OnImpact(Transform projectile, Collider hit)
        {
            var damageble = hit.GetComponent<IDamageable>();
            if (damageble != null)
            {
                Vector3 hitDirection = projectile.forward;
                Vector3 hitPoint = hit.ClosestPoint(projectile.position);
                damageble.TakeDamage(damage, hitDirection, hitPoint);
            }
            
        }
    }
}