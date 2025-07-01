using System;
using MyMaterials.Scripts.Entity.Player;
using UnityEngine;
using UnityEngine.UI;

namespace MyMaterials.Scripts.UI
{
    public class HUDManager : MonoBehaviour
    {
        // [Header("Reticle References")]
        // [SerializeField] private Image reticleImage;
        // [SerializeField] private Image lockOnBoxUI;

        [Header("SliderReferences")]
        [SerializeField] private Slider boosSlider;
        [SerializeField] private Slider healthSlider;
        
        // [Header("Reticle Settings")]
        // [SerializeField] private Color reticleNormalColor = Color.white;
        // [SerializeField] private Color reticleMainLockOnColor = Color.red;
        // [SerializeField] private float reticleTransitionDuration = 0.2f;
        
        [Header("Slider Settings")]
        [SerializeField] private float boostEnergyMax;
        
        private Vector2 defaultPivotPosReticleImage;
        private Vector2 defaultPivotPosLockOnBoxUI;
        private float currentBoostEnergyValue;
        
        
        /*----外部コンポーネントの参照----*/
        private LockOnManager lockOnManager;
        private Camera mainCamera;
        
        /*----イベント購読対象の参照----*/
        private PlayerHealth playerHealth;
        
        private void Start()
        {
            playerHealth = FindObjectOfType<PlayerHealth>();

            if (playerHealth != null)
            {
                // PlayerHealthのHP更新イベント購読とHPゲージの初期化
                playerHealth.OnHealthChanged += UpdateHealthUI;

                UpdateHealthUI(playerHealth.CurrentHealth, playerHealth.MaxHealth);
            }
            else
            {
                Debug.LogWarning("HUDManager: シーン内にPlayerHealthが見つかりませんでした。");
            }
            
            if (boosSlider != null)
            {
                boosSlider.maxValue = boostEnergyMax;
                boosSlider.minValue = 0f;
            }

            SetBoostEnergyValue(boostEnergyMax);
        }

        private void OnDestroy()
        {
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged -= UpdateHealthUI;
            }
        }

        private void UpdateHealthUI(float currentHealth, float maxHealth)
        {
            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }
        }

        public void SetBoostEnergyValue(float newValue)
        {
            currentBoostEnergyValue = Mathf.Clamp(newValue, 0, boostEnergyMax);

            if (boosSlider != null)
            {
                boosSlider.value = currentBoostEnergyValue;
            }
        }
    }
}