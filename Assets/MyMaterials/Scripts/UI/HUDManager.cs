using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HUDManager : MonoBehaviour
    {
        [SerializeField] private Slider boosSlider;
        [SerializeField] private Slider hitPointSlider;

        [SerializeField] private float boostEnergyMax;

        private float currentBoostEnergyValue;
        
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