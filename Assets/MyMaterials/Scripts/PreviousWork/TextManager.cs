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
    [SerializeField] private TMP_Text playerSpeedXText;
    [SerializeField] private TMP_Text playerSpeedYText;
    [SerializeField] private TMP_Text playerSpeedZText;

    private GameObject player;
    private Rigidbody playerRigidBody;

    private void Awake()
    {
        player = GameObject.Find("Player");
        if (player != null)
        {
            playerRigidBody = player.GetComponent<Rigidbody>();
        }
    }

    private void Start()
    {
        scoreText.text = "0";
        absorbedEnergyText.text = "0";
        playerSpeedXText.text = "0.00";
        playerSpeedYText.text = "0.00";
        playerSpeedZText.text = "0.00";
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

    private void Update()
    {
        if (playerRigidBody != null)
        {
            playerSpeedXText.text = playerRigidBody.velocity.x.ToString("F2");
            playerSpeedYText.text = playerRigidBody.velocity.y.ToString("F2");
            playerSpeedZText.text = playerRigidBody.velocity.z.ToString("F2");
        }
    }
}