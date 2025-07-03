using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using MyMaterials.Scripts.Singletons;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Weapons
{
    public class EnemyWeapons:MonoBehaviour
    {
        [Header("基本設定")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float fireForce = 300;
        
        
        [Header("攻撃性能")] 
        [SerializeField] private float attackInterval = 2f;
        [SerializeField] private int burstCount = 3;
        [SerializeField] private float timeBetWeenShots = 0.1f;


        private bool isFiring = false;

        /// <summary>
        /// AIから攻撃の開始を試みるために呼び出される
        /// </summary>
        public void TryFire()
        {
            if (isFiring) return;
            
            FireSequenceAsync().Forget();
        }
        
        /// <summary>
        /// バースト射撃から次の攻撃のクールダウンまでの一連の流れを管理する非同期メソッド
        /// </summary>
        private async UniTaskVoid FireSequenceAsync()
        {
            // 攻撃処理を開始
            isFiring = true;
            
            // オブジェクトが破壊されたときにタスクを自動でキャンセルするためのトークン
            var token = this.GetCancellationTokenOnDestroy();

            try
            {
                // バースト射撃を実行
                for (int i = 0; i < burstCount; i++)
                {
                    if(token.IsCancellationRequested) break;
                    
                    FireSingleShot();
                    
                    await UniTask.Delay(TimeSpan.FromSeconds(timeBetWeenShots), cancellationToken: token);
                }
                
                // 次の攻撃までのクールダウン
                await UniTask.Delay(TimeSpan.FromSeconds(attackInterval), cancellationToken: token);
            }
            finally
            {
                isFiring = false;
            }
        }
        
        /// <summary>
        /// 弾を一発だけ発射する
        /// </summary>
        public void FireSingleShot()
        {
            if(bulletPrefab == null || firePoint == null) return;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.AddForce(firePoint.forward * fireForce, ForceMode.Impulse);
                AudioManager.Instance.PlaySE(SoundType.BeamGun_1);
            }
        }
    }
}