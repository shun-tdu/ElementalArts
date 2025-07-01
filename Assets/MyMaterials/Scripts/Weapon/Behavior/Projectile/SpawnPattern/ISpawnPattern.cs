using System;
using UnityEngine;

namespace MyMaterials.Scripts.Weapon.Behavior.Projectile.SpawnPattern
{
    /// <summary>
    /// 弾の発射パターンを定義するインターフェイス
    /// </summary>
    public interface ISpawnPattern
    {
        /// <summary>
        /// 弾を発射する
        /// </summary>
        /// <param name="muzzle">銃口のTransform</param>
        /// <param name="projectilePrefab">生成する弾のプレハブ</param>
        /// <param name="onProjectileSpawned">弾が1つ生成される度に呼ばれるコールバック</param>
        void Spawn(Transform muzzle, GameObject projectilePrefab, Action<GameObject> onProjectileSpawned);
    }
}