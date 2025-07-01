using UnityEngine;
using MyMaterials.Scripts.Weapon.Behavior.Projectile.Core;
using MyMaterials.Scripts.Weapon.Behavior.Projectile.Impact;

namespace MyMaterials.Scripts.Weapon.Behavior.Projectile.Movement
{
    [CreateAssetMenu(menuName = "Weapons/Behavior/Projectiles/Movement/Homing")]
    public class HomingMovement : ScriptableObject, IProjectileMovement
    {
        [SerializeField] private float turnSpeed;
        [SerializeField] private float initialSpeed;
        [SerializeField] private string targetTag = "Enemy";
        // private Transform target;

        public void Setup(Rigidbody rb, IProjectileImpact impact, Transform lockOnTarget)
        {
            Transform finalTarget;

            if (lockOnTarget != null)
            {
                finalTarget = lockOnTarget;
            }
            else
            {
                finalTarget = FindClosestEnemy(rb.transform.position);
            }
            
            
            // HomingProjectile コンポーネントにターゲットを渡す
            var homing = rb.GetComponent<HomingProjectile>();
            if (homing == null) return;

            homing.Initialize(impact, finalTarget, turnSpeed);
        }

        private Transform FindClosestEnemy(Vector3 fromPosition)
        {
            var targets = GameObject.FindGameObjectsWithTag(targetTag);
            Transform closestTarget = null;
            float closestDistance = Mathf.Infinity;
            
            foreach (var t in targets)
            {
                float distance = Vector3.Distance(t.transform.position, fromPosition);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = t.transform;
                }
            }
            
            return closestTarget;
        }
    }
}