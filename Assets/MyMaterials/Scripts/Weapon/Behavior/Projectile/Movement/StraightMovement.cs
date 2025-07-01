using UnityEngine;
using MyMaterials.Scripts.Weapon.Behavior.Projectile.Movement;

namespace MyMaterials.Scripts.Weapon.Behavior.Projectile.Movement
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