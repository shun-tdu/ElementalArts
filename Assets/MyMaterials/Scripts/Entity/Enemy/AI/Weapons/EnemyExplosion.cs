using MyMaterials.Scripts.Entity.Enemy.AI.Core;
using UnityEngine;
using MyMaterials.Scripts.Singletons;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Weapons
{
    public class EnemyExplosion : MonoBehaviour
    {
        [Header("自爆攻撃の設定")]
        [SerializeField] private float attackRadius = 10f;
        [SerializeField] private float damage = 100f;
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private LayerMask obstacleMask;
        
        /// <summary>
        /// Enemyの自爆攻撃
        /// Enemy中心の半径 attackRadius 内のIDamageableインターフェースを持つオブジェクトに
        /// damage だけの攻撃をする
        /// </summary>
        public void Explode()
        {
            EffectManager.Instance.PlayEffect(EffectType.Explosion_1, transform.position, Quaternion.identity);
            AudioManager.Instance.PlaySE(SoundType.Explosion_1);
            
            Collider[] targetInAttackRadius = Physics.OverlapSphere(transform.position, attackRadius, targetMask);
            
            foreach (var targetCollider in targetInAttackRadius)
            {
                // ターゲットとの間に障害物がないかチェック
                Vector3 dirToTarget = (targetCollider.transform.position - transform.position).normalized;
                float distToTarget = Vector3.Distance(transform.position, targetCollider.transform.position);
                
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    //ダメージを受けられるオブジェクトかをチェック
                    if (targetCollider.TryGetComponent<IDamageable>(out var damageable))
                    {
                        Vector3 hitPoint = targetCollider.ClosestPoint(transform.position);

                        Vector3 hidDirection = (targetCollider.transform.position - transform.position).normalized;

                        damageable?.TakeDamage(damage, hitPoint, hidDirection);
                    }
                }
            }

            var controller = GetComponent<EnemyAIController>();
            controller?.NotifyDeath();
            
            Destroy(gameObject);
        }
    }
}