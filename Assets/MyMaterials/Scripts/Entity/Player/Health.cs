using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class Health : MonoBehaviour
    {
        public float maxHP;
        public float currentHp { get; private set; }

        void Awake() => currentHp = maxHP;

        public void TakeDamage(float damage)
        {
            currentHp = Mathf.Max(currentHp - damage, 0f);
            Debug.Log(currentHp);
            //todo 死亡処理を追加
            // if (currentHp <= 0f) onDeath.Invoke();
        }
    }
}