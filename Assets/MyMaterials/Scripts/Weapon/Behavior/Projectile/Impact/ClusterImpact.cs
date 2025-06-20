using UnityEngine;

namespace weapon.Behavior.Projectile.Impact
{
    [CreateAssetMenu(menuName = "Weapons/Behavior/Projectiles/Impact/Cluster")]
    public class ClusterImpact : ScriptableObject, IProjectileImpact
    {
        public GameObject subProjectilePrefab;
        public int count;
        public float spreadAngle;

        public void OnImpact(Transform projectile ,Collider hit)
        {
            for (int i = 0; i < count; i++)
            {
                //分裂用弾を生成
            }
            Destroy(hit.gameObject);
        }
    }
}