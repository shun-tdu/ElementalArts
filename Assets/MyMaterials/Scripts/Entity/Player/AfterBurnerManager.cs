using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Player
{
    public class AfterBurnerManager : MonoBehaviour
    {
        [SerializeField] private VisualEffect afterBurnerVFX;

        [SerializeField] private string speedProp = "Speed";
        [SerializeField] private string intensityProp = "Intensity";


        private Rigidbody playerRigidBody;

        private void Awake()
        {
            playerRigidBody = GameObject.Find("Player").GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            float playerSpeed = playerRigidBody.velocity.magnitude / 30f + 0.2f;
            afterBurnerVFX.SetFloat(intensityProp,playerSpeed);
            afterBurnerVFX.SetFloat(speedProp,playerSpeed);
        }
    }

}
