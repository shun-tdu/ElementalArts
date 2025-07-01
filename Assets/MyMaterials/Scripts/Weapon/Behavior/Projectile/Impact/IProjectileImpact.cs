using UnityEngine;

namespace MyMaterials.Scripts.Weapon.Behavior.Projectile.Impact
{
    // 着弾時の動作を定義するインターフェイス
    public interface IProjectileImpact
    {
        void OnImpact(Transform projectile ,Collider hit);
    }
}