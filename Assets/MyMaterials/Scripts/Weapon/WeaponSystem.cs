using System;
using Cysharp.Threading.Tasks;
using MyMaterials.Scripts.Weapon.Behavior.Projectile;
using UnityEngine;

namespace MyMaterials.Scripts.Weapon
{
    public class WeaponSystem : MonoBehaviour
    {
        [Header("Behavior")] [SerializeField] private WeaponBehavior behavior;

        [Header("Hardware")] [SerializeField] private Transform muzzlePoint;
        
        // ---- 内部状態 ----
        private Transform currentTarget;
        public float CurrentAmmo { get; private set; }
        public bool IsReloading { get; private set; } = false;
        
        // ---- UI通知用のイベント ----
        public event Action<float, float> OnAmmoChanged;    // 残弾数変化を通知
        public event Action<bool> OnReloadStatsChanged; // リロード状態変化を通知


        private void Start()
        {
            // 起動時に最大までリロード
            Reload(true);
        }
        
        
        /// <summary>
        /// Weaponにターゲットを設定する
        /// </summary>
        public void SetLockOnTarget(Transform target)
        {
            this.currentTarget = target;
        }

        /// <summary>
        /// 押した瞬間の処理を受け取る
        /// </summary>
        public void OnTriggerDown(Transform muzzle)
        {
            if (CurrentAmmo > 0 && !IsReloading)
            {
                behavior?.OnTriggerDown(this, muzzlePoint, currentTarget);
            }
        }

        /// <summary> 長押し中毎フレームの処理を受け取る </summary>
        public void OnTriggerHold(Transform muzzle)
        {
            if (CurrentAmmo > 0 && !IsReloading)
            {
                behavior?.OnTriggerHold(this, muzzlePoint, currentTarget);
            } 
        }

        /// <summary> 離した瞬間の処理を受け取る </summary>
        public void OnTriggerUp(Transform muzzle)
        {
            behavior?.OnTriggerUp(this, muzzlePoint, currentTarget);
        }
        
        /// <summary>
        /// 弾薬を消費するメソッド
        /// イベント経由でUIへの弾数変化通知も行う
        /// </summary>
        public void ConsumeAmmo(float amount = 1)
        {
            CurrentAmmo -= amount;
            if (CurrentAmmo <= 0) CurrentAmmo = 0;
            
            // UIに残弾数の変化を通知
            float maxAmmo = GetMaxAmmo();
            OnAmmoChanged?.Invoke(CurrentAmmo, maxAmmo);
            
        }
        
        /// <summary>
        /// リロードを開始する
        /// リロード可能なら非同期処理を開始し、リロードを行う
        /// </summary>
        public void TryReload()
        {
            // 既にリロード中 or 弾が満タンの場合は何もしない
            if(IsReloading) return;

            if (behavior is IMagazineWeapon magazineWeapon)
            {
                if(CurrentAmmo >= magazineWeapon.MagazineSize) return;
                
                ReloadAsync(magazineWeapon.ReloadTime).Forget();
            }
        }
        
        /// <summary>
        /// 指定した時間でリロードを処理を完了する非同期関数
        /// </summary>
        /// <param name="duration"></param>
        private async UniTaskVoid ReloadAsync(float duration)
        {
            Debug.Log("リロード開始...");
            IsReloading = true;
            
            // UIにリロード開始を通知
            OnReloadStatsChanged?.Invoke(true); 

            await UniTask.Delay(TimeSpan.FromSeconds(duration),
                cancellationToken: this.GetCancellationTokenOnDestroy());
            
            // 弾を装填
            Reload(false);
        }
        
        
        /// <summary>
        /// リロードを行う
        /// 各種リロード処理の最後に呼ばれ、弾数をIMagazineWeapon
        /// </summary>
        /// <param name="isInitial"></param>
        private void Reload(bool isInitial)
        {
            float maxAmmo = GetMaxAmmo();
            CurrentAmmo = maxAmmo;
            IsReloading = false;

            if (!isInitial) 
            {
                OnReloadStatsChanged?.Invoke(false);
            }
            
            OnAmmoChanged?.Invoke(CurrentAmmo, maxAmmo); 
            Debug.Log("リロード完了");
        }

        
        /// <summary>
        /// 現在の武器の最大弾薬/エネルギー量を取得するヘルパー関数
        /// </summary>
        private float GetMaxAmmo()
        {
            if (behavior is IMagazineWeapon magazineWeapon)
            {
                return magazineWeapon.MagazineSize;
            }

            if (behavior is IChargeWeapon chargeWeapon)
            {
                return chargeWeapon.MaxCharge;
            }

            return 0;
        }
    }
}