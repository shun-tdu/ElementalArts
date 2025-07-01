using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Weapons
{
    public class EnemyWeapons:MonoBehaviour
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float fireForce = 300;

        [Header("バースト射撃の設定")] 
        [SerializeField] private int burstCount = 3;
        [SerializeField] private float timeBetWeenShots = 0.1f;

        private bool isFiring = false;

        public void StartBurstFire()
        {
            if(isFiring) return;

            FireBurstAsync().Forget();
        }

        private async UniTaskVoid FireBurstAsync()
        {
            isFiring = true;

            var token = this.GetCancellationTokenOnDestroy();

            try
            {
                for (int i = 0; i < burstCount; i++)
                {
                    if (token.IsCancellationRequested) break;

                    FireSingleShot();

                    await UniTask.Delay(TimeSpan.FromSeconds(timeBetWeenShots), cancellationToken: token);
                }
            }
            finally
            {
                isFiring = false;
            }
        }
        
        public void FireSingleShot()
        {
            if(bulletPrefab == null || firePoint == null) return;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.AddForce(firePoint.forward * fireForce, ForceMode.Impulse);
            }
        }
    }
}