using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


namespace Environment
{
    [RequireComponent(typeof(VisualEffect))]
    public class DustSpawner : MonoBehaviour
    {
        [Header("VFX 設定")]
        [SerializeField] string velocityProp = "VelocityOffset";  // Graph 側と一致させる
        [SerializeField] float strengthFactor = 1.0f;            // 速度を何倍にするか

        // [Header("Player")]
        // [SerializeField] 
        private Rigidbody playerRigidbody;                       // プレイヤー Transform

        VisualEffect vfx;
        Vector3 lastPlayerPos;

        void Awake()
        {
            vfx = GetComponent<VisualEffect>();
            playerRigidbody = GameObject.Find("Player").GetComponent<Rigidbody>();
        }

        void Update()
        {
            // 1. プレイヤーのワールド移動ベクトルを取得
            Vector3 worldVel = playerRigidbody.velocity;

            // 2. ワールド→ローカル座標に変換
            //    これをやらないと、回転時に軸が狂ってしまう
            Vector3 localVel = transform.InverseTransformDirection(worldVel);

            // 3. 逆向きにして strengthFactor を掛け、VFX Graph に渡す
            vfx.SetVector3(velocityProp, -localVel * strengthFactor);
        }
    }
}
