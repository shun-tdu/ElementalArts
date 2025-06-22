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

        [Header("Aiming Reticle UI")] 
        [SerializeField] private Image reticleImage;

        [SerializeField] private Color reticleNormalColor;
        [SerializeField] private Color reticleMainLockOnColor = Color.red;
        [SerializeField] private Color reticleSubLockOnColor = Color.yellow;
        [SerializeField] private float defaultAimDistance = 100f;
        
        
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
        private CameraManager camManager;
        private LockOnManager lockOnManager;
        private Rigidbody rb;
        private Transform emitPoint;
        // private float yaw, pitch;



        private void Awake()
        {
            //コンポーネントの初期化
            rb = GetComponent<Rigidbody>();
            // cameraPivot = transform.Find("CameraPivot");
            emitPoint = transform.Find("EmitPoint");
            controls = new PlayerControls();
            weapon = GetComponent<WeaponSystem>();
            camManager = GetComponent<CameraManager>();
            lockOnManager = GetComponent<LockOnManager>();
            
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
            controls.Player.Fire.canceled += _ => weapon.OnTriggerUp(emitPoint);

            //----LockOnとAim----
            controls.Player.LockOn.performed += _ => lockOnManager.ToggleLockOn(this.transform);
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
            camManager.UpdateCameraState(transform, rb.velocity);

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
            //した2つは同じUpdateで呼ばないとカクつく
            //移動処理
            HandleMovement();
            
            //キャラクターの向きをカメラに合わせる処理
            HandleCharacterRotation();
            
            if (controls.Player.Fire.IsPressed())
            {
                weapon.OnTriggerHold(emitPoint);
                weapon.SetLockOnTarget(lockOnManager.GetCurrentTarget());
            }
        }
        
        /// <summary>
        /// Playerのターゲットを指定するメソッド
        /// </summary>
        /// <param name="target">ターゲットのTransform</param>
        public void SetTarget(Transform target)
        {
            weapon.SetLockOnTarget(target);
        }
        
        
        private void HandleCharacterRotation()
        {
            var horizontalRotation = Quaternion.AngleAxis(horizontalAim.Value, Vector3.up);
            var verticalRotation = Quaternion.AngleAxis(verticalAim.Value, Vector3.right);
            transform.rotation = horizontalRotation;
            camManager.EyeTransform.localRotation = verticalRotation;
            
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
        /// 移動処理をまとめた関数
        /// </summary>
        private void HandleMovement()
        {

            Vector3 moveDirection= transform.right * moveRawInput.x + transform.forward * moveRawInput.y;
            
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
            Transform target = lockOnManager.GetCurrentTarget();
            weapon.OnTriggerDown(emitPoint);
            weapon.SetLockOnTarget(target);
        }
        

        /// <summary>
        /// レティクル関連の処理
        /// </summary>
        private void UpdateReticle()
        {
            if (reticleImage == null || Camera.main == null) return;

            //----サブロックオン状態(最優先)----
            if (lockOnManager.IsAimingSubTarget &&  lockOnManager.SubLockOnTarget != null)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(lockOnManager.SubLockOnTarget.position);

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
            else if (lockOnManager.MainLockOnTarget != null)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(lockOnManager.MainLockOnTarget.position);

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
                // Ray ray = new Ray(cameraPivot.position, cameraPivot.forward);
                Ray ray = new Ray(camManager.EyeTransform.position,  camManager.EyeTransform.forward);   
                Vector3 targetPoint;

                // レイがロックオン対象レイヤーの何かに当たったら
                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, lockOnManager.LockOnLayer))
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