using Player;
using UnityEngine;
using UnityEngine.UI;
using Player;
using Cysharp.Threading.Tasks;

namespace UI
{
    public class HUDManager : MonoBehaviour
    {
        // [Header("Reticle References")]
        // [SerializeField] private Image reticleImage;
        // [SerializeField] private Image lockOnBoxUI;

        [Header("SliderReferences")]
        [SerializeField] private Slider boosSlider;
        [SerializeField] private Slider hitPointSlider;
        
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
        
        
        private void Awake()
        {
            if (boosSlider != null)
            {
                boosSlider.maxValue = boostEnergyMax;
                boosSlider.minValue = 0f;
            }

            SetBoostEnergyValue(boostEnergyMax);
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