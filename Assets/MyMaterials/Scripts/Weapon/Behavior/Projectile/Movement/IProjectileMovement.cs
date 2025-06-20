using UnityEngine;

namespace weapon.Behavior.Projectile.Movement
{
    // 飛び方を定義するインターフェイス
    public interface IProjectileMovement
    {
        void Setup(Rigidbody rb, Impact.IProjectileImpact impact, Transform target);
    }
}