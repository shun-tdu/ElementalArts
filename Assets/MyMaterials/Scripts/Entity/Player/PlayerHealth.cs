using System;
using System.Collections;
using System.Collections.Generic;
using MyMaterials.Scripts.Managers.Singletons;
using UnityEngine;

namespace MyMaterials.Scripts.Entity.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        /// <summary>
        /// HP変化を外部に通知するイベント
        /// 第一引数：現在のHP、第二引数：最大HP 
        /// </summary>
        public event Action<float, float> OnHealthChanged;
        
        /// <summary>
        /// ダメージを受けたときに、その方向を通知するイベント 
        /// </summary>
        public event Action<Vector3> OnDamaged; 
        
        [Tooltip("プレイヤーHP")]
        [field:SerializeField] public float MaxHealth { get; private set; } = 100f;
        
        public float CurrentHealth { get; private set; }

        private void Start()
        {
            CurrentHealth = MaxHealth;
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }
        
        
        /// <summary>
        /// 被弾処理
        /// </summary>
        public void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
        {
            CurrentHealth = Mathf.Max(CurrentHealth - damage, 0f);
            
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
            OnDamaged?.Invoke(hitDirection);
            
            EffectManager.Instance.PlayEffect(EffectType.HitEffect_1, transform.position, Quaternion.identity);
            AudioManager.Instance.PlaySE(SoundType.HitEffect_1);
            
            if (CurrentHealth <= 0f)
            {
                Die();
            }
        }
        
        /// <summary>
        /// プレイヤー死亡時の処理
        /// </summary>
        private void Die()
        {
            Debug.Log("プレイヤーは力尽きた");
        }
    }
}