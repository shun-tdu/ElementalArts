using System;
using UnityEngine;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyMovement:MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float rotationSpeed = 5f;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }
        
        /// <summary>
        /// 指定されたターゲットの方向へ移動する
        /// </summary>
        public void MoveTowards(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
            LockAt(targetPosition);
        }
        
        /// <summary>
        /// 指定されたターゲットの方向を滑らかに向く
        /// </summary>
        public void LockAt(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lockRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lockRotation, Time.deltaTime * rotationSpeed);
            }
        }
        
        /// <summary>
        /// 索敵中の行動（ゆっくりと回転するなど）
        /// </summary>
        public void Patrol()
        {
            transform.Rotate(0, 20f * Time.deltaTime, 0);
            Stop();
        }

        /// <summary>
        /// 移動を停止する
        /// </summary>
        public void Stop()
        {
            rb.velocity = Vector3.zero;
        }
    }
}