using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Movement
{
    public class EnemyStrafeMovement : MonoBehaviour
    {
        [Header("移動性能")] 
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float rotationSpeed = 10f;

        [Header("ストレイフ設定")] 
        [SerializeField] private float strafeDirectionChangeInterval = 3.0f;

        private Rigidbody rb;
        private float strafeTimer;
        private int strafeDirection = 1;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            // ランダムなストレイフ方向に初期化
            strafeDirection = Random.value > 0.5f? 1:-1;
        }

        public void Strafe(Transform target)
        {
            if(target == null) return;
            
            // ---- 向きの制御 ----
            // 常にターゲットを向き続ける制御
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            
            // ---- 移動の制御 ----
            // タイマーを更新し、一定時間ごとに回り込む方向を反転させる
            strafeTimer += Time.deltaTime;
            if (strafeTimer > strafeDirectionChangeInterval)
            {
                strafeDirection *= -1;
                strafeTimer = 0f;
            }
            
            // ターゲットに対して水平な方向に移動
            Vector3 strafeVector = transform.right * strafeDirection;
            rb.velocity = strafeVector * moveSpeed;
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
        /// 索敵中の行動
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