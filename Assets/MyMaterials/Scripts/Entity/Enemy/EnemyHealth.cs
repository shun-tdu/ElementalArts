using System;
using UnityEngine;

namespace MyMaterials.Scripts.Entity.Enemy
{
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        [Header("HP設定")] 
        [SerializeField] private float maxHealth = 300f;
        
        public float CurrentHealth { get; private set; }

        public event Action OnDeath;

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
        {
            if(CurrentHealth <= 0) return;

            CurrentHealth -= damage;
            
            // ここで被弾エフェクトをhitPositionに出したり、被弾音を鳴らしたりする
            // EffectManager.Instance.PlayHitEffect(hitPosition);
            // AudioManager.Instance.PlaySEAtPoint(SoundType.EnemyHit, hitPosition);


            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Debug.Log($"{gameObject.name}は倒された。");
            
            OnDeath?.Invoke();
            
            // ここで死亡エフェクトを再生したり、アイテムをドロップしたりする
            // EffectManager.Instance.PlayExplosionEffect(transform.position);

            Destroy(gameObject);
        }
    }
}