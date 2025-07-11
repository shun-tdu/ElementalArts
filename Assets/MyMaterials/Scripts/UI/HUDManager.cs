﻿using System;
using MyMaterials.Scripts.Entity.Player;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using MyMaterials.Scripts.Entity.Enemy;
using MyMaterials.Scripts.Weapon;


namespace MyMaterials.Scripts.UI
{
    public class HUDManager : MonoBehaviour
    {
        [Header("HP & Boost Sliders")]
        [SerializeField] private Slider boosSlider;
        [SerializeField] private Slider healthSlider;
        // [SerializeField] private float boostEnergyMax;
        
        [Header("Lock-On UI References")]
        [SerializeField] private Image reticleImage;
        [SerializeField] private Image lockOnBoxUI;
        [SerializeField] private Image apertureGaugeLeft;
        [SerializeField] private Image apertureGaugeRight;
        [SerializeField] private Image aperturePartHorizon;
        [SerializeField] private Image aperturePartVertical;
        [SerializeField] private Image gaugeTop;
        [SerializeField] private Image gaugeLeft;
        [SerializeField] private Image gaugeRight;
        
        [Header("Lock-On UI Settings")]
        [SerializeField] private Color reticleNormalColor;
        [SerializeField] private Color reticleLockOnColor;
        [SerializeField] private Color lockOnBoxNormalColor;
        [SerializeField] private Color lockOnBoxRockOnColor;
        [SerializeField] private float reticleTransitionDuration = 0.2f;
        
        [Header("Gauge Colors")]
        [SerializeField] private Color normalGaugeColor = Color.white;
        [SerializeField] private Color reloadingGaugeColor = Color.red;
        
        // ---- 内部参照 ----
        private PlayerHealth playerHealth;
        private LockOnManager lockOnManager;
        private PlayerController playerController;
        private Camera mainCamera;
        private Vector2 defaultPivotPosReticleImage;
        private Vector2 defaultPivotPosLockOnBoxUI;
        private WeaponSystem activeMainWeapon;
        private WeaponSystem activeSubWeapon;
        
        private float mainWeaponStartFill;  //リロード開始時のゲージ量を記憶するための変数
        private float subWeaponStartFill;   //リロード開始時のゲージ量を記憶するための変数
        
        
        private void Start()
        {
            mainCamera = Camera.main;
            playerHealth = FindObjectOfType<PlayerHealth>();
            lockOnManager = FindObjectOfType<LockOnManager>();
            playerController = FindObjectOfType<PlayerController>();
            
            // ---- プレイヤーのHPイベント購読 ----
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged += UpdateHealthUI;
                UpdateHealthUI(playerHealth.CurrentHealth, playerHealth.MaxHealth);
            }

            // ---- ブーストゲージの初期化 ----
            if (boosSlider != null)
            {
                boosSlider.maxValue = playerController.EnergyMax;
                boosSlider.value = playerController.EnergyMax;
            }
            
            // ---- ロックオンUIの初期化とイベント購読 ----
            if (lockOnManager != null)
            {
                defaultPivotPosReticleImage = reticleImage.rectTransform.pivot;
                defaultPivotPosLockOnBoxUI = lockOnBoxUI.rectTransform.pivot;
                lockOnManager.OnStateChanged += HandleLockOnStateChange;
                HandleLockOnStateChange(lockOnManager.CurrentState);
            }
            
            // ---- プレイヤーの武器切り替えイベントの購読 ----
            if (playerController != null)
            {
                playerController.OnActiveWeaponChanged += HandleActiveWeaponsChanged;
                HandleActiveWeaponsChanged(playerController.GetMainWeapon(), playerController.GetSubWeapon());
            }
        }

        private void OnDestroy()
        {
            // イベント購読を解除 (メモリリーク防止)
            if (playerHealth != null) playerHealth.OnHealthChanged -= UpdateHealthUI;
            if (lockOnManager != null) lockOnManager.OnStateChanged -= HandleLockOnStateChange;
            if(playerController != null) playerController.OnActiveWeaponChanged -= HandleActiveWeaponsChanged;
            
            // 監視中の武器のイベントも解除
            if (activeMainWeapon != null)
            {
                activeMainWeapon.OnAmmoChanged -= UpdateMainWeaponAmmoUI;
                activeMainWeapon.OnReloadStatusChanged -= HandleMainWeaponReloadStatus;
                activeMainWeapon.OnReloadProgress -= UpdateMainWeaponReloadProgress;
            }

            if (activeSubWeapon != null)
            {
                activeSubWeapon.OnAmmoChanged -= UpdateSubWeaponAmmoUI;
                activeSubWeapon.OnReloadStatusChanged -= HandleSubWeaponReloadStatus; // ★追加
                activeSubWeapon.OnReloadProgress -= UpdateSubWeaponReloadProgress; // ★追加
            }
        }

        
        private void LateUpdate()
        {
            // ロックオン中は毎フレームUIを更新
            if (lockOnManager != null && lockOnManager.IsLockedOn && lockOnManager.MainLockOnTarget != null)
            {
                UpdateLockOnUIPosition();
                UpdateGauges();
            }
        }

        
        public void SetEnergyValue(float newValue)
        {
            if (boosSlider != null)
            {
                boosSlider.value = Mathf.Clamp(newValue, 0, playerController.EnergyMax);
            }
        }
        
        
        /// <summary>
        /// LockOnManagerからの状態変化通知を受けてUIを更新する
        /// </summary>
        private void HandleLockOnStateChange(LockOnState newState)
        {
            switch (newState)
            {
                case LockOnState.Free:
                    reticleImage.gameObject.SetActive(true);
                    reticleImage.color = reticleNormalColor;
                    lockOnBoxUI.gameObject.SetActive(true);
                    lockOnBoxUI.color = lockOnBoxNormalColor;
                    MoveUIToCenterAsync(reticleImage, defaultPivotPosReticleImage, reticleTransitionDuration).Forget();
                    MoveUIToCenterAsync(lockOnBoxUI, defaultPivotPosLockOnBoxUI, reticleTransitionDuration).Forget();
                    SetAperture(1f);
                    SetGauge(1f, gaugeTop);
                    break;
                case LockOnState.Weak:
                    reticleImage.color = reticleLockOnColor;
                    break;
                case LockOnState.Intention:
                    reticleImage.color = reticleLockOnColor;
                    lockOnBoxUI.color = lockOnBoxRockOnColor;
                    break;
            }
        }
        
        
        /// <summary>
        /// ロックオンUIの位置をターゲットに追従させる
        /// </summary>
        private void UpdateLockOnUIPosition()
        {
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(lockOnManager.MainLockOnTarget.position);

            if (screenPosition.z > 0 && 
                screenPosition.x > 0 &&
                screenPosition.x < Screen.width && 
                screenPosition.y > 0 && 
                screenPosition.y < Screen.height)
            {
                reticleImage.gameObject.SetActive(true);
                reticleImage.transform.position = screenPosition;

                if (lockOnManager.CurrentState == LockOnState.Intention)
                {
                    lockOnBoxUI.gameObject.SetActive(true);
                    lockOnBoxUI.transform.position = screenPosition;
                }
                else
                {
                    lockOnBoxUI.gameObject.SetActive(true);
                }
            }
            else
            {
                reticleImage.gameObject.SetActive(false);
                lockOnBoxUI.gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// ロックオン中の各種ゲージを更新する
        /// </summary>
        private void UpdateGauges()
        {
            // 絞りゲージ
            SetAperture(1f - lockOnManager.MainTargetDistance / lockOnManager.LockOnRange);

            // ターゲットのHPゲージ
            if (lockOnManager.CurrentState == LockOnState.Intention && lockOnManager.MainLockOnTarget.TryGetComponent<EnemyHealth>(out var enemyHealth))
            {
                SetGauge(enemyHealth.CurrentHealth / enemyHealth.MaxHealth, gaugeTop);
            }
        }
        
        
        /// <summary>
        /// 指定されたUI要素を、時間をかけて画面中央へスムーズに移動させる
        /// </summary>
        /// <param name="img">移動させるImage</param>
        /// <param name="defaultPivotPos">初期状態のPivot位置</param>
        /// <param name="duration">移動にかける時間（秒）</param>
        private async UniTaskVoid MoveUIToCenterAsync(Image img, Vector2 defaultPivotPos, float duration)
        {
            var token = img.GetCancellationTokenOnDestroy();
            
            //移動開始時の位置を取得
            Vector3 startPos = img.rectTransform.position;
            //目標値計算
            Vector3 endPos = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

            float elapsedTime = 0f;
            
            while (elapsedTime < duration)
            {
                //経過時間に基づいて位置を線形補間
                img.rectTransform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
                
                //時間を更新
                elapsedTime += Time.deltaTime;
                
                //次フレームまで待機
                await UniTask.Yield(cancellationToken: token);
            }
            
            if(token.IsCancellationRequested) return;
            ResetUIToCenter(img, defaultPivotPos);
        }
        
        
        /// <summary>
        /// レティクルを初期化する
        /// 徐々に中心に戻す
        /// </summary>
        /// <param name="img">初期化するImage</param>
        private void ResetUIToCenter(Image img, Vector2 pivotPos)
        {
            RectTransform rt = img.rectTransform;
            rt.anchorMin = rt.anchorMax = rt.pivot = pivotPos;
            rt.anchoredPosition = Vector2.zero;
        }
        
        
        /// <summary>
        /// Apertureを指定した割合に即した表示にする
        /// 入力は表示割合
        /// </summary>
        private void SetAperture(float amount)
        {
            float fillAmount = Mathf.Clamp(amount, 0f, 1f);
            apertureGaugeLeft.fillAmount = Mathf.Clamp(amount, 0f, 1f);
            apertureGaugeRight.fillAmount = Mathf.Clamp(amount, 0f, 1f);

            aperturePartVertical.rectTransform.localEulerAngles = new Vector3(0f, 0f, (1 - amount) * 90f);
        }
        
        
        /// <summary>
        /// レティクルの指定したゲージを指定した割合に設定する
        /// </summary>
        private void SetGauge(float amount, Image gaugeImage)
        {
            amount = Mathf.Clamp(amount, 0f, 1f);
            gaugeImage.fillAmount = amount;
        }
        
        private void UpdateHealthUI(float currentHealth, float maxHealth)
        {
            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }
        }

        /// <summary>
        /// PlayerControllerからの武器切り替え通知を受けて、監視対象を更新する
        /// </summary>
        private void HandleActiveWeaponsChanged(WeaponSystem newMain, WeaponSystem newSub)
        {
            // 古い武器のイベント監視を解除
            if (activeMainWeapon != null)
            {
                activeMainWeapon.OnAmmoChanged -= UpdateMainWeaponAmmoUI;
                activeMainWeapon.OnReloadStatusChanged -= HandleMainWeaponReloadStatus;
                activeMainWeapon.OnReloadProgress -= UpdateMainWeaponReloadProgress;
                gaugeLeft.color = normalGaugeColor;
            }
            if (activeSubWeapon != null)
            {
                activeSubWeapon.OnAmmoChanged -= UpdateSubWeaponAmmoUI;
                activeSubWeapon.OnReloadStatusChanged -= HandleSubWeaponReloadStatus;
                activeSubWeapon.OnReloadProgress -= UpdateSubWeaponReloadProgress;
                gaugeRight.color = normalGaugeColor;
            }
            
            // 新しい武器への参照を保持
            activeMainWeapon = newMain;
            activeSubWeapon = newSub;
            
            // 新しい武器のイベント監視を開始
            if (activeMainWeapon != null)
            {
                activeMainWeapon.OnAmmoChanged += UpdateMainWeaponAmmoUI;
                activeMainWeapon.OnReloadStatusChanged += HandleMainWeaponReloadStatus;
                activeMainWeapon.OnReloadProgress += UpdateMainWeaponReloadProgress;
                
                UpdateMainWeaponAmmoUI(activeMainWeapon.CurrentAmmo, activeMainWeapon.GetMaxAmmo());
                
                if (activeMainWeapon.IsReloading)
                {
                    HandleMainWeaponReloadStatus(true);
                    UpdateMainWeaponReloadProgress(activeMainWeapon.CurrentReloadProgress);
                }
            }
            else
            {
                SetGauge(0, gaugeLeft);
            }
            
            if (activeSubWeapon != null)
            {
                activeSubWeapon.OnAmmoChanged += UpdateSubWeaponAmmoUI;
                activeSubWeapon.OnReloadStatusChanged += HandleSubWeaponReloadStatus;
                activeSubWeapon.OnReloadProgress += UpdateSubWeaponReloadProgress;
                
                UpdateSubWeaponAmmoUI(activeSubWeapon.CurrentAmmo, activeSubWeapon.GetMaxAmmo());

                if (activeSubWeapon.IsReloading)
                {
                    HandleSubWeaponReloadStatus(true);
                    UpdateSubWeaponReloadProgress(activeSubWeapon.CurrentReloadProgress);
                }
            }
            else
            {
                SetGauge(0, gaugeRight);
            }
        }
        
        
        /// <summary>
        /// メイン武器のリロード状態を管理
        /// </summary>
        private void HandleMainWeaponReloadStatus(bool isReloading)
        {
            if (isReloading)
            {
                // リロード開始時のゲージ量を記憶し、色を変える
                mainWeaponStartFill = activeMainWeapon.CurrentAmmo / activeMainWeapon.GetMaxAmmo();
                gaugeLeft.color = reloadingGaugeColor;
            }
            else
            {
                // リロード完了時は色を戻す
                gaugeLeft.color = normalGaugeColor;
            }
        }

        
        /// <summary>
        /// サブ武器のリロード状態を管理
        /// </summary>
        private void HandleSubWeaponReloadStatus(bool isReloading)
        {
            if (isReloading)
            {
                subWeaponStartFill = activeSubWeapon.CurrentAmmo / activeSubWeapon.GetMaxAmmo();
                gaugeRight.color = reloadingGaugeColor;
            }
            else
            {
                gaugeRight.color = normalGaugeColor;
            }
        }
        
        /// <summary>
        /// メイン武器のリロード進捗を更新
        /// </summary>
        private void UpdateMainWeaponReloadProgress(float progress)
        {
            // 開始時のゲージ量から満タン(1.0)までを、進捗(progress)に応じて補間する
            float targetFill = Mathf.Lerp(mainWeaponStartFill, 1.0f, progress);
            SetGauge(targetFill, gaugeLeft);
        }

        
        /// <summary>
        ///　サブ武器のリロード進捗を更新
        /// </summary>
        private void UpdateSubWeaponReloadProgress(float progress)
        {
            float targetFill = Mathf.Lerp(subWeaponStartFill, 1.0f, progress);
            SetGauge(targetFill, gaugeRight);
        }
        
        
        /// <summary>
        /// メイン武器の弾薬変化を受けて、左ゲージを更新する
        /// </summary>
        private void UpdateMainWeaponAmmoUI(float currentAmmo, float maxAmmo)
        {
            if (maxAmmo > 0)
            {
                SetGauge(currentAmmo / maxAmmo, gaugeLeft);
            }
            else
            {
                SetGauge(0, gaugeLeft);
            }
        }
        
        /// <summary>
        /// サブ武器の弾薬変化を受けて、右ゲージを更新する
        /// </summary>
        private void UpdateSubWeaponAmmoUI(float currentAmmo, float maxAmmo)
        {
            if (maxAmmo > 0)
            {
                SetGauge(currentAmmo / maxAmmo, gaugeRight);
            }
            else
            {
                SetGauge(0, gaugeRight);
            }
        }
    }
}