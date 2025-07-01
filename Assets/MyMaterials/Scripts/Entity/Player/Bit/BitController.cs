using System;
using UnityEngine;
using UnityEngine.VFX;

namespace Player.Bit
{
    public class BitController : MonoBehaviour
    {
        [Header("追従設定 (Follow Settings)")] 
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Vector3 formationOffset;
        [SerializeField] private float positionSmoothTime = 0.1f;
        [SerializeField] private float rotationSmoothTime = 0.2f;
        
        [Header("ジェット噴射エフェクト")]
        [SerializeField] private VisualEffect afterBurnerVFX;
        [SerializeField] private string speedProp = "Speed";
        [SerializeField] private string intensityProp = "Intensity";
        [SerializeField] private float normalIntensity = 1f;
        [SerializeField] private float thrustIntensity = 2f;
        
        
        // ----内部変数---- //
        private Vector3 positionVelocity;
        
        // PlayerControllerから毎フレーム受け取る情報
        private Vector3 thrustInput;

        /// <summary>
        /// PlayerControllerからビットの状態を更新するために呼び出される
        /// </summary>
        public void UpdateBitState(Vector3 currentThrustInput)
        {
            this.thrustInput = currentThrustInput;
        }

        private void FixedUpdate()
        {
            if(playerTransform == null) return;
            
            // 毎フレームの位置処理
            HandlePosition();

            // 毎フレームの回転処理
            HandleRotation();

            // 毎フレームのVFX処理
            HandleJetEffect();
        }
        
        /// <summary>
        /// ビットの位置を計算する
        /// </summary>
        private void HandlePosition()
        {
            Vector3 targetPosition = playerTransform.position + playerTransform.rotation * formationOffset;

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref positionVelocity,
                positionSmoothTime);
        }
        
        /// <summary>
        /// ビットの向きを制御する
        /// </summary>
        private void HandleRotation()
        {
            Quaternion targetRotation;

            if (thrustInput.sqrMagnitude > 0.01f)
            {
                Vector3 thrustDirection = -thrustInput.normalized;
                targetRotation = Quaternion.LookRotation(thrustDirection, playerTransform.up);
            }
            else
            {
                targetRotation = Quaternion.LookRotation(playerTransform.forward, -playerTransform.up);
            }

            transform.rotation =
                Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime / rotationSmoothTime);
        }

        private void HandleJetEffect()
        {
            if(afterBurnerVFX == null) return;

            float thrustPower = thrustInput.magnitude;

            if (thrustPower > 0.1f)
            {
                afterBurnerVFX.SetFloat(intensityProp, thrustIntensity);
                afterBurnerVFX.SetFloat(speedProp, thrustIntensity);
            }
            else
            {
                afterBurnerVFX.SetFloat(intensityProp, normalIntensity);
                afterBurnerVFX.SetFloat(speedProp, normalIntensity);
            }
        }
    }
}