using System;
using System.Collections;
using System.Collections.Generic;
using MyMaterials.Scripts.Managers.Singletons;
using UnityEngine;

namespace MyMaterials.Scripts.Entity.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        // HP変化を外部に通知するイベント
        // 第一引数：現在のHP、第二引数：最大HP
        public event Action<float, float> OnHealthChanged;

        [field:SerializeField] public float MaxHealth { get; private set; } = 100f;
        
        public float CurrentHealth { get; private set; }

        private void Start()
        {
            CurrentHealth = MaxHealth;
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }

        public void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
        {
            CurrentHealth = Mathf.Max(CurrentHealth - damage, 0f);
            
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
            
            EffectManager.Instance.PlayEffect(EffectType.HitEffect_1, transform.position, Quaternion.identity);
            AudioManager.Instance.PlaySE(SoundType.HitEffect_1);
            
            //todo 死亡処理を追加
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