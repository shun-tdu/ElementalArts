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
        [SerializeField] private float accelTime = 0.2f;
        [SerializeField] private float decelTime = 0.5f;
        private Vector3 currentVelocity;
        
        [Header("Look")] 
        [SerializeField] public float lookSensitivity = 2f;

        [Header("Lock-On")] 
        [SerializeField] private float lockOnRange = 100f;   //ロックオン可能な最大距離
        [SerializeField] private float lockOnAngle = 30f;    //画面中心からこの角度内を探索
        [SerializeField] private LayerMask lockOnLayer;      //ロックオン対照のレイヤー

        [Header("Aiming Reticle UI")] 
        [SerializeField] private Image reticleImage;

        [SerializeField] private Color reticleNormalColor;
        [SerializeField] private Color reticleMainLockOnColor = Color.red;
        [SerializeField] private Color reticleSubLockOnColor = Color.yellow;
        [SerializeField] private float defaultAimDistance = 100f;


        [Header("Cinemachine Setting")] 
        [SerializeField] private Transform eyeTransform;
        [SerializeField] private CinemachineVirtualCamera vCamFreeLookLow;
        [SerializeField] private CinemachineVirtualCamera vCamFreeLookHighLeft;
        [SerializeField] private CinemachineVirtualCamera vCamFreeLookHighRight;
        [SerializeField] private float speedThreshold;
        [SerializeField] private float horizontalSpeedThreshold;
        
        // [SerializeField] private CinemachineTargetGroup targetGroup;

        [Header("Input Control(Lock Axes)")] 
        public AxisState horizontalAim;
        public AxisState verticalAim;
        
        /*========内部状態変数========*/
        //操作系入力値
        private Vector2 moveRawInput = Vector2.zero;
        // private Vector2 lookInput = Vector2.zero;

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
        private Transform mainLockOnTarget;
        private Transform subLockOnTarget;
        private bool isAimingSubTarget = false;
        private bool isLockedOn = false;

        private void Awake()
        {
            //コンポーネントの初期化
            rb = GetComponent<Rigidbody>();
            cameraPivot = transform.Find("CameraPivot");
            controls = new PlayerControls();
            weapon = GetComponent<WeaponSystem>();

            //イベントの購読
            //----Movement----
            controls.Player.Move.performed += ctx => moveRawInput = ctx.ReadValue<Vector2>();
            controls.Player.Move.canceled += ctx => moveRawInput = Vector2.zero;

            //----Lock----
            // controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
            // controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;

            //----Thrust----
            controls.Player.ThrustUp.performed += ctx => isThrustUp = true;
            controls.Player.ThrustUp.canceled += ctx => isThrustUp = false;
            controls.Player.ThrustDown.performed += ctx => isThrustDown = true;
            controls.Player.ThrustDown.canceled += ctx => isThrustDown = false;

            //----WeaponSystem----
            controls.Player.Fire.started += _ => FireWeapon();
            controls.Player.Fire.canceled += _ => weapon.OnTriggerUp(cameraPivot);

            //----LockOnとAim----
            // controls.Player.LockOn.performed += _ => ToggleLockOn();
            // controls.Player.Aim.started += ctc => StartAimingSubTarget();
            // controls.Player.Aim.canceled += ctc => StopAimingSubTarget();

            // yaw = transform.eulerAngles.y;
            // pitch = cameraPivot.localEulerAngles.x;
        }

        private void Start()
        {
            //----カーソルの画面固定処理----
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnEnable() => controls.Player.Enable();
        private void OnDisable() => controls.Player.Disable();

        private void Update()
        {
            //視線の更新処理
            // HandleLook();

            //カメラステートの更新処理
            UpdateCameraState();

            //カーソル固定処理
            HandleCursorLock();
            
            //レティクルの更新処理
            UpdateReticle();
            
            //AxisStateの更新処理
            horizontalAim.Update(Time.deltaTime);
            verticalAim.Update(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            //した３つは同じUpdateで呼ばないとカクつく
            //移動処理
            HandleMovement();
            

            
            //キャラクターの向きをカメラに合わせる処理
            HandleCharacterRotation();
            
            if (controls.Player.Fire.IsPressed())
            {
                weapon.OnTriggerHold(cameraPivot);
                weapon.SetLockOnTarget(GetCurrentTarget());
            }
        }
        

        private void HandleCharacterRotation()
        {
            if (!isLockedOn)
            {
                var horizontalRotation = Quaternion.AngleAxis(horizontalAim.Value, Vector3.up);
                var verticalRotation = Quaternion.AngleAxis(verticalAim.Value, Vector3.right);
                transform.rotation = horizontalRotation;
                eyeTransform.localRotation = verticalRotation;
            }
            // if (isLockedOn && mainLockOnTarget != null)
            // {
            //     Vector3 directionToTarget = mainLockOnTarget.position - transform.position;
            //     directionToTarget.y = 0;
            //     if (directionToTarget.sqrMagnitude > 0.01f)
            //     {
            //         Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            //         transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            //     }
            // }
            // else
            // {
            //     Transform camTransform = Camera.main.transform;
            //     Vector3 camForward = camTransform.forward;
            //     camForward.y = 0;
            //
            //     if (camForward.sqrMagnitude > 0.01)
            //     {
            //         transform.rotation = Quaternion.LookRotation(camForward);
            //     }
            // }
        }

        /// <summary>
        /// 視線処理をまとめた関数
        /// </summary>
        // private void HandleLook()
        // {
        //     //メインロックオン時はキャラクターが常にターゲットの方向を向く
        //     if (mainLockOnTarget != null && !isAimingSubTarget)
        //     {
        //         Vector3 directionToTarget = mainLockOnTarget.position - transform.position;
        //         directionToTarget.y = 0;
        //         Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        //         transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        //         yaw = transform.eulerAngles.y;
        //     }
        //     //通常時とサブエイム時はマウス操作
        //     else
        //     {
        //         yaw += lookInput.x * lookSensitivity;
        //         transform.rotation = Quaternion.Euler(0, yaw, 0);
        //     }
        //
        //     pitch -= lookInput.y * lookSensitivity;
        //     pitch = Mathf.Clamp(pitch, -80f, 80f);
        //     cameraPivot.localRotation = Quaternion.Euler(pitch, 0, 0);
        // }

        /// <summary>
        /// 移動処理をまとめた関数
        /// </summary>
        private void HandleMovement()
        {

            Vector3 moveDirection;

            if (isLockedOn && mainLockOnTarget != null)
            {
                Vector3 forwardToTarget = mainLockOnTarget.position - transform.position;
                forwardToTarget.y = 0;
                forwardToTarget.Normalize();
                Vector3 rightToTarget = Vector3.Cross(Vector3.up, forwardToTarget);
                moveDirection = rightToTarget * moveRawInput.x + forwardToTarget * moveRawInput.y;
            }
            else
            {
                moveDirection = transform.right * moveRawInput.x + transform.forward * moveRawInput.y;
            }
            
            //目標速度計算
            Vector3 targetVel = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z)* moveSpeed;
            
            //加減速の判定
            Vector2 current2D = new Vector2(rb.velocity.x, rb.velocity.z);
            Vector2 target2D = new Vector2(targetVel.x, targetVel.z);
            bool isAccelerating = target2D.sqrMagnitude > current2D.sqrMagnitude;
            float smoothTime = isAccelerating ? accelTime : decelTime;
            
            //加減速に応じたsmoothTimeで加減速
            Vector3 smoothedVel = Vector3.SmoothDamp(
                rb.velocity,
                targetVel,
                ref currentVelocity,
                smoothTime
                );
            rb.velocity = smoothedVel;
            
            //垂直スラスタ系
            if (isThrustUp) rb.AddForce(Vector3.up * thrustAccel, ForceMode.Acceleration);
            if (isThrustDown) rb.AddForce(Vector3.down * thrustAccel, ForceMode.Acceleration);
        }

        /// <summary>
        /// 攻撃処理をまとめた関数
        /// </summary>
        private void FireWeapon()
        {
            //todo 暫定処理でTriggerDown時にターゲットを渡す処理にしている 不具合があればOnTriggerDownでターゲットを渡す
            Transform target = GetCurrentTarget();
            weapon.OnTriggerDown(cameraPivot);
            weapon.SetLockOnTarget(target);
        }

        /// <summary>
        /// 現在ロックオン中のターゲットを返す
        /// </summary>
        private Transform GetCurrentTarget()
        {
            return isAimingSubTarget ? subLockOnTarget : mainLockOnTarget;
        }


        /// <summary>
        /// カメラステートの更新
        /// </summary>
        private void UpdateCameraState()
        {
            Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
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

        // private void ToggleLockOn()
        // {
        //     if (isLockedOn)
        //     {
        //         isLockedOn = false;
        //         mainLockOnTarget = null;
        //         UpdateTargetGroup();
        //         weapon.SetLockOnTarget(null);
        //     }
        //     else
        //     {
        //         FindBestTarget();
        //         if (mainLockOnTarget != null)
        //         {
        //             isLockedOn = true;
        //             UpdateTargetGroup();
        //             weapon.SetLockOnTarget(mainLockOnTarget);
        //         }
        //     }
        // }

        // private void UpdateTargetGroup()
        // {
        //     if (mainLockOnTarget != null)
        //     {
        //         // ターゲットグループのメンバーをプレイヤーとメインターゲットの2つに設定
        //         targetGroup.m_Targets = new CinemachineTargetGroup.Target[2]
        //         {
        //             new CinemachineTargetGroup.Target { target = this.transform, weight = 1, radius = 1 },
        //             new CinemachineTargetGroup.Target { target = mainLockOnTarget, weight = 1, radius = 1 }
        //         };
        //     }
        //     else
        //     {
        //         targetGroup.m_Targets = new CinemachineTargetGroup.Target[1]
        //         {
        //             new CinemachineTargetGroup.Target { target = this.transform, weight = 1, radius = 1 }
        //         };
        //     }
        // }

        private void StartAimingSubTarget()
        {
            isAimingSubTarget = true;
            if (mainLockOnTarget != null)
                subLockOnTarget = mainLockOnTarget;
        }

        private void StopAimingSubTarget()
        {
            isAimingSubTarget = false;
        }
        
        
        /// <summary>
        /// Playerから球状の領域を発生
        /// 領域内の敵をリストアップ
        /// 最も近い敵をmainLockOnTargetに設定
        /// </summary>
        private void FindBestTarget()
        {
            //シーン内のすべての敵候補を検索
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

            mainLockOnTarget = bestTarget;
            if (mainLockOnTarget != null)
            {
                Debug.Log($"ロックオン: {mainLockOnTarget.name}");
            }
        }

        /// <summary>
        /// レティクル関連の処理
        /// </summary>
        private void UpdateReticle()
        {
            if (reticleImage == null || Camera.main == null) return;

            //----サブロックオン状態(最優先)----
            if (isAimingSubTarget && subLockOnTarget != null)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(subLockOnTarget.position);

                // ターゲットがカメラの前方にあり、かつ画面内にいるかチェック
                if (screenPosition.z > 0 &&
                    screenPosition.x > 0 && screenPosition.x < Screen.width &&
                    screenPosition.y > 0 && screenPosition.y < Screen.height)
                {
                    reticleImage.enabled = true; // 表示する
                    reticleImage.transform.position = screenPosition; // ターゲットに追従
                    reticleImage.color = reticleSubLockOnColor; // サブロックオン用の色に
                }
                else
                {
                    reticleImage.enabled = false; // 画面外なら非表示
                }
            }
            // --- 2. メインロックオン状態 ---
            else if (mainLockOnTarget != null)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(mainLockOnTarget.position);

                if (screenPosition.z > 0 &&
                    screenPosition.x > 0 && screenPosition.x < Screen.width &&
                    screenPosition.y > 0 && screenPosition.y < Screen.height)
                {
                    reticleImage.enabled = true;
                    reticleImage.transform.position = screenPosition; // ターゲットに追従
                    reticleImage.color = reticleMainLockOnColor; // メインロックオン用の色に
                }
                else
                {
                    reticleImage.enabled = false;
                }
            }
            // --- 3. フリー（非ロックオン）状態 ---
            else
            {
                reticleImage.enabled = true;
                reticleImage.color = reticleNormalColor; // 通常の色に

                // レイをカメラの中心から正面に飛ばす
                Ray ray = new Ray(cameraPivot.position, cameraPivot.forward);
                Vector3 targetPoint;

                // レイがロックオン対象レイヤーの何かに当たったら
                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, lockOnLayer))
                {
                    targetPoint = hit.point; // 当たった地点を照準のターゲットとする
                }
                else
                {
                    targetPoint = ray.GetPoint(defaultAimDistance); // 何にも当たらなかったら、遠方をターゲットとする
                }

                // 3Dのターゲット座標を2Dのスクリーン座標に変換してレティクルを移動
                reticleImage.transform.position = Camera.main.WorldToScreenPoint(targetPoint);
            }
        }

        /// <summary>
        /// カーソルの更新処理
        /// </summary>
        private void HandleCursorLock()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = false;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (Cursor.lockState != CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = false;
                }
            }
        }
    }
}