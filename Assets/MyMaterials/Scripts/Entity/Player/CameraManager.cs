using UnityEngine;
using Cinemachine;

namespace Player
{
    public class CameraManager:MonoBehaviour
    {
        [Header("Cinemachine Setting")] 
        [Header("Normal Camera")] 
        [SerializeField] private CinemachineVirtualCamera vCamFreeLookLow;
        [SerializeField] private CinemachineVirtualCamera vCamFreeLookHighLeft;
        [SerializeField] private CinemachineVirtualCamera vCamFreeLookHighRight;
        [SerializeField] private float speedThreshold;
        [SerializeField] private float horizontalSpeedThreshold;

        [Header("LockOn Camera")] 
        [SerializeField] private CinemachineVirtualCamera vCamMainLockOn;
        [SerializeField] private CinemachineTargetGroup targetGroup;
        [SerializeField] private float maxLockCameraDistance = 20f; //引きカメラとノーマルカメラ切り替え距離
        
        
        /// <summary>
        /// カメラステートの更新
        /// Playerの速度に応じてカメラを切り替える
        /// </summary>
        public void UpdateCameraState(Vector3 playerVelocity)
        {
            Vector3 localVelocity = transform.InverseTransformDirection(playerVelocity);
            float localRightSpeed = localVelocity.x;
            float localForwardSoeed = localVelocity.z;
            float speed = new Vector2(localRightSpeed, localForwardSoeed).magnitude;
            
            //低速時は画面中央に機体を表示
            if (speed < speedThreshold)
            {
                Debug.Log("State:Low");
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
                    Debug.Log("State:Right");

                    vCamFreeLookLow.Priority = 11;
                    vCamFreeLookHighLeft.Priority = 10;
                    vCamFreeLookHighRight.Priority = 12;
                }
                //左向き移動時のカメラ配置
                else if(localRightSpeed < -horizontalSpeedThreshold)
                {
                    Debug.Log("State:Left");

                    vCamFreeLookLow.Priority = 11;
                    vCamFreeLookHighLeft.Priority = 12;
                    vCamFreeLookHighRight.Priority = 10;
                }
                //正面姑息時のカメラ配置
                else
                {
                    Debug.Log("State:ForwardHigh");
                    vCamFreeLookLow.Priority = 11;
                    vCamFreeLookHighLeft.Priority = 12;
                    vCamFreeLookHighRight.Priority = 10;
                }
            }
        }
    }
}