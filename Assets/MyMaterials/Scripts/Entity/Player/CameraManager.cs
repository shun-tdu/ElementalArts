using System;
using UnityEngine;
using Cinemachine;

namespace MyMaterials.Scripts.Entity.Player
{
    public class CameraManager : MonoBehaviour
    {
        [Header("Cinemachine Setting")] [Header("Normal Camera")] [SerializeField]
        private CinemachineVirtualCamera vCamFreeLookLow;

        [SerializeField] private CinemachineVirtualCamera vCamFreeLookHighLeft;
        [SerializeField] private CinemachineVirtualCamera vCamFreeLookHighRight;
        [SerializeField] private float speedThreshold;
        [SerializeField] private float horizontalSpeedThreshold;
        
        //スクリーンの中心Transform
        public Transform EyeTransform { get; private set; }


        //コンポーネントの内部参照
        private LockOnManager lockOnManager;
        
        private void Awake()
        {
            EyeTransform = GameObject.Find("Eye").transform;
            lockOnManager = GetComponent<LockOnManager>();
        }

        /// <summary>
        /// カメラステートの更新
        /// Playerの速度に応じてカメラを切り替える
        /// </summary>
        public void UpdateCameraState(Transform playerTransform, Vector3 playerVelocity)
        {
            Vector3 localVelocity = playerTransform.InverseTransformDirection(playerVelocity);
            float localRightSpeed = localVelocity.x;
            float localForwardSpeed = localVelocity.z;
            float speed = new Vector2(localRightSpeed, localForwardSpeed).magnitude;


            //低速時は画面中央に機体を表示
            if (speed < speedThreshold)
            {
                // Debug.Log("低速時");
                vCamFreeLookLow.Priority = 12;
                vCamFreeLookHighLeft.Priority = 11;
                vCamFreeLookHighRight.Priority = 10;
            }
            //高速移動時はPlayerの左右速度に応じてカメラを切り替え
            else
            {
                //右向き移動時のカメラ配置
                if (localRightSpeed > horizontalSpeedThreshold)
                {
                    // Debug.Log("高速右");
                    vCamFreeLookLow.Priority = 11;
                    vCamFreeLookHighLeft.Priority = 10;
                    vCamFreeLookHighRight.Priority = 12;
                }
                //左向き移動時のカメラ配置
                else if (localRightSpeed < -horizontalSpeedThreshold)
                {
                    // Debug.Log("高速左");
                    vCamFreeLookLow.Priority = 11;
                    vCamFreeLookHighLeft.Priority = 12;
                    vCamFreeLookHighRight.Priority = 10;
                }
                //正面姑息時のカメラ配置
                else
                {
                    // Debug.Log("高速高速");
                    vCamFreeLookLow.Priority = 11;
                    vCamFreeLookHighLeft.Priority = 12;
                    vCamFreeLookHighRight.Priority = 10;
                }
            }
        }
    }
}

