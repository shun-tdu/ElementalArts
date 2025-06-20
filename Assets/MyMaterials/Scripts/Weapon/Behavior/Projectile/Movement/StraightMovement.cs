using UnityEngine;
using weapon.Behavior.Projectile.Movement;

namespace weapon.Behavior.Projectile.Movement
{
    [CreateAssetMenu(menuName = "Weapons/Behavior/Projectiles/Movement/Straight")]
    public class StraightMovement : ScriptableObject, IProjectileMovement
    {
        public void Setup(Rigidbody rb, Impact.IProjectileImpact impact, Transform target)
        {
            //何もしない or　最初の力を与える
        }
    }
}