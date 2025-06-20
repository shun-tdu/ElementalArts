using System;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

using weapon;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")] 
        [SerializeField] public float moveSpeed = 10f;
        [SerializeField] public float thrustAccel = 30f;

        [Header("Look")] 
        [SerializeField] public float lookSensitivity = 2f;

        [Header("Lock-On")] 
        [SerializeField] private float lockOnRange = 100f;  //ロックオン可能な最大距離
        [SerializeField] private float lockOnAngle = 30f;   //画面中心からこの角度内を探索
        [SerializeField] private LayerMask lockOnLayer;     //ロックオン対照のレイヤー

        [Header("Aiming UI")] 
        [SerializeField] private Image reticleImage;
        [SerializeField] private Color reticleNormalColor = Color.white;
        [SerializeField] private Color reticleLockOnlColor = Color.red;
        [SerializeField] private float defaultAimDistance = 100f; 
        
        
        //操作系入力値
        private Vector2 moveInput = Vector2.zero;
        private Vector2 lookInput = Vector2.zero;
        private bool isThrustUp = false;
        private bool isThrustDown = false;
        private bool isFireSecondaryWeapon = false;

        //アタッチされているコンポーネントの内部参照
        private PlayerControls controls;
        private WeaponSystem weapon;
        private Rigidbody rb;
        private Transform cameraPivot;
        private float yaw, pitch;
        
        //ロックオン関連変数
        private Transform currentLockOnTarget;
        
        //Cinemachine関連変数
        private CinemachineVirtualCamera playerVirtualCamera;
        private CinemachineBasicMultiChannelPerlin cameraNoise;
        
        private void Awake()
        {
            //コンポーネントの初期化
            rb = GetComponent<Rigidbody>();
            cameraPivot = transform.Find("CameraPivot");
            controls = new PlayerControls();
            weapon = GetComponent<WeaponSystem>();

            //イベントの購読
            //----Movement----
            controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
            
            //----Lock----
            controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
            controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;
            
            //----Thrust----
            controls.Player.ThrustUp.performed += ctx => isThrustUp = true;
            controls.Player.ThrustUp.canceled += ctx => isThrustUp = false;
            controls.Player.ThrustDown.performed += ctx => isThrustDown = true;
            controls.Player.ThrustDown.canceled += ctx => isThrustDown = false;
            
            //----WeaponSystem----
            controls.Player.Fire.started += _ => weapon.OnTriggerDown(cameraPivot);
            controls.Player.Fire.canceled += _ => weapon.OnTriggerUp(cameraPivot);
            
            // controls.Player.SeondaryFire.performed += weapon.FireSecondary;

            controls.Player.LockOn.performed += _ => ToggleLockOn();
            
            yaw = transform.eulerAngles.y;
            pitch = cameraPivot.localEulerAngles.x;

            playerVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            if (playerVirtualCamera != null)
            {
                cameraNoise = playerVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            }
        }

        private void OnEnable() => controls.Player.Enable();
        private void OnDisable() => controls.Player.Disable();

        private void Update()
        {
            //マウスルック
            // yaw += lookInput.x * lookSensitivity;
            // pitch -= lookInput.y * lookSensitivity;
            // pitch = Mathf.Clamp(pitch, -80f, 80f);
            //
            // transform.rotation = Quaternion.Euler(0, yaw, 0);
            // cameraPivot.localRotation = Quaternion.Euler(pitch, 0, 0);
            
            UpdateReticle();
        }

        private void FixedUpdate()
        {
            //----水平移動----
            Vector3 inputH = new Vector3(moveInput.x, 0, moveInput.y);
            Vector3 desireVel = transform.TransformDirection(inputH) * moveSpeed;
            Vector3 velChange = desireVel - rb.velocity;
            velChange.y = 0;
            rb.AddForce(velChange, ForceMode.VelocityChange);

            //----垂直スラスタ----
            if (isThrustUp) rb.AddForce(Vector3.up * thrustAccel, ForceMode.Acceleration);
            if (isThrustDown) rb.AddForce(Vector3.down * thrustAccel, ForceMode.Acceleration);
            
            //----ホールド検知----
            if(controls.Player.Fire.ReadValue<float>() > 0.5f)
                weapon.OnTriggerHold(cameraPivot);
            
            //----カメラシェイクの制御----
            if (cameraNoise != null)
            {
                if (isThrustUp || isThrustDown)
                {
                    cameraNoise.m_AmplitudeGain = 2f;
                }
                else
                {
                    cameraNoise.m_AmplitudeGain = 0f;
                }
            }
            
        }

        private void ToggleLockOn()
        {
            // if (currentLockOnTarget == null)
            // {
            //     FindBestTarget();
            // }
            // else
            // {
            //     currentLockOnTarget = null;
            //     // Debug.Log("ロックオン解除");
            // }
            
            FindBestTarget();
            
            if (weapon != null)
            {
                weapon.SetLockOnTarget(currentLockOnTarget);
            }
        }

        private void FindBestTarget()
        {
            //シーンなにのすべての敵候補を検索
            Collider[] potentialTargets = Physics.OverlapSphere(transform.position, lockOnRange, lockOnLayer);

            Transform bestTarget = null;
            float minScreenDistance = float.MaxValue;

            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

            foreach (var targetCollider in potentialTargets)
            {
                Vector3 directionToTarget = targetCollider.transform.position - cameraPivot.position;

                if (Vector3.Angle(cameraPivot.forward, directionToTarget) < lockOnAngle)
                {
                    Vector3 screenPoint = Camera.main.WorldToScreenPoint(targetCollider.transform.position);
                    if (screenPoint.z > 0)
                    {
                        float distance = Vector2.Distance(screenPoint, screenCenter);
                        if (distance < minScreenDistance)
                        {
                            minScreenDistance = distance;
                            bestTarget = targetCollider.transform;
                        }
                    }
                }
            }

            currentLockOnTarget = bestTarget;
            if (currentLockOnTarget != null)
            {
                Debug.Log($"ロックオン: {currentLockOnTarget.name}");
            }
        }
        
        /// <summary>
        /// レティクル関連の処理
        /// </summary>
        private void UpdateReticle()
        {
            if(reticleImage == null) return;

            if (currentLockOnTarget != null)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(currentLockOnTarget.position);

                if (screenPosition.z > 0 &&
                    screenPosition.x > 0 && screenPosition.x < Screen.width &&
                    screenPosition.y > 0 && screenPosition.y < Screen.height)
                {
                    reticleImage.enabled = true;

                    reticleImage.transform.position = screenPosition;
                    reticleImage.color = reticleLockOnlColor;
                }
                else
                {
                    reticleImage.enabled = false;
                }
            }
            else
            {
                reticleImage.enabled = true;
                reticleImage.color = reticleNormalColor;

                Ray ray = new Ray(cameraPivot.position, cameraPivot.forward);
                Vector3 targetPoint;
                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, lockOnLayer))
                {
                    targetPoint = hit.point;
                }
                else
                {
                    targetPoint = ray.GetPoint(defaultAimDistance);
                }

                Vector2 screenPosition = Camera.main.WorldToScreenPoint(targetPoint);
                reticleImage.transform.position = screenPosition;
            }
        }
    }
}