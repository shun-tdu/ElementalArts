using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyMaterials.Scripts.Entity.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        // HP変化を外部に通知するイベント
        // 第一引数：現在のHP、第二引数：最大HP
        public event Action<float, float> OnHealthChanged;
        
        [SerializeField]private float maxHealth = 100f;
        public float MaxHealth => maxHealth;
        public float CurrentHealth { get; private set; }

        private void Start()
        {
            CurrentHealth = maxHealth;
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        public void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
        {
            CurrentHealth = Mathf.Max(CurrentHealth - damage, 0f);

            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
            
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