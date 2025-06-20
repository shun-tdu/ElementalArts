using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI playerLifeText;
    [SerializeField] private TextMeshProUGUI absorbedEnergyText;

    private void Start()
    {
        scoreText.text = "0";
        absorbedEnergyText.text = "0";
    }

    public void ChangeScore(float score)
    {
        scoreText.text = score.ToString();
    }

    public void ChangePlayerLife(float playerLife)
    {
        playerLifeText.text = playerLife.ToString();
    }

    public void ChangeAbsorbedEnergy(float absorbedEnergy)
    {
        absorbedEnergyText.text = absorbedEnergy.ToString();
    }
}