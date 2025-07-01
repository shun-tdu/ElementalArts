using System;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

using MyMaterials.Scripts.Weapon;
using MyMaterials.Scripts.UI;
using MyMaterials.Scripts.Entity.Player.Bit;
using MyMaterials.Scripts.Singletons;

namespace MyMaterials.Scripts.Entity.Player
{
    public enum BitFormationState
    {
        Idle,
        Forward,
        Backward,
        Left,
        Right,
        Up,
        Down
    }
    
    [System.Serializable]
    public class FormationMapping
    {
        public BitFormationState state;
        public BitFormationData formationData;
    }
    
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")] 
        [SerializeField] public float moveSpeed = 10f;
        [SerializeField] public float thrustAccel = 30f;
        [SerializeField] private float accelTime = 0.2f;
        [SerializeField] private float decelTime = 0.5f;
        
        [Header("Step Settings")] 
        [SerializeField] private float stepForce = 15f;     //ダッシュのインパルス強度
        [SerializeField] private float stepCoolDown = 1.0f; //ダッシュのクールダウン

        [Header("Dash Settings")] 
        [SerializeField] private float dashSpeed = 80f;
        [SerializeField] private float dashEnergyMax = 100f;
        [SerializeField] private float dashEnergyConsume = 10f;
        [SerializeField] private float dashEnergyRegeneration = 20f;
        [SerializeField] private float dashHoldThreshold = 0.2f;
        [SerializeField] private float dashAccelTime = 1.0f;
        private float currentDashEnergy;
        private float dashHoldTime = 0f;
        private bool isDashing = false;
        private bool isDashHeld = false;
        
        [Header("Look")] 
        [SerializeField] public float lookSensitivity = 2f;
        
        [Header("Input Control(Lock Axes)")] 
        public AxisState horizontalAim;
        public AxisState verticalAim;

        [Header("ビット制御 (Bit Control)")]
        [SerializeField] private BitFormationManager bitManager; // BitFormationManagerへの参照
        [SerializeField] private List<FormationMapping> formationMappings; // 状態と設計図のマッピングリスト

        
        /*========内部状態変数========*/
        //操作系入力値
        private Vector2 moveRawInput = Vector2.zero;

        private bool isThrustUp = false;
        private bool isThrustDown = false;
        private bool isFireSecondaryWeapon = false;
        private bool canStep = true;
        private Vector3 currentVelocity;
        
        //ビット制御系
        private Dictionary<BitFormationState, BitFormationData> formationDictionary;
        private BitFormationState currentBitState;

        //SE制御系
        private bool isEngineSoundPlaying = false;
        
        //アタッチされているコンポーネントの内部参照
        private PlayerControls controls;
        private WeaponSystem weapon;
        private CameraManager camManager;
        private LockOnManager lockOnManager;
        private HUDManager hudManager;
        private Rigidbody rb;
        private Transform emitPoint;
        
        //ロックオン状態の外部参照
        public InputAction LockOnAction => controls.Player.LockOn;
        

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
            hudManager = GameObject.Find("Canvas/InGameHUD").GetComponent<HUDManager>();

            formationDictionary = new Dictionary<BitFormationState, BitFormationData>();
            foreach (var mapping in formationMappings)
            {
                formationDictionary[mapping.state] = mapping.formationData;
            }
            
            //イベントの購読
            //----Movement----
            controls.Player.Move.performed += ctx => moveRawInput = ctx.ReadValue<Vector2>();
            controls.Player.Move.canceled += ctx => moveRawInput = Vector2.zero;

            //----Thrust----
            controls.Player.ThrustUp.performed += ctx => isThrustUp = true;
            controls.Player.ThrustUp.canceled += ctx => isThrustUp = false;
            controls.Player.ThrustDown.performed += ctx => isThrustDown = true;
            controls.Player.ThrustDown.canceled += ctx => isThrustDown = false;
            
            //----Step----
            controls.Player.Step.performed += ctx => TryStep();
            
            //----Dash----
            controls.Player.Dash.performed += _ => StartDashHold();
            controls.Player.Dash.canceled += _ => EndDashHold();
            
            
            //----WeaponSystem----
            controls.Player.Fire.started += _ => FireWeapon();
            controls.Player.Fire.canceled += _ => weapon.OnTriggerUp(emitPoint);
        }

        private void Start()
        {
            //----カーソルの画面固定処理----
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            currentDashEnergy = dashEnergyMax;
        }

        private void OnEnable() => controls.Player.Enable();
        private void OnDisable() => controls.Player.Disable();

        private void Update()
        {
            //カメラステートの更新処理
            camManager.UpdateCameraState(transform, rb.velocity);
            
            //AxisStateの更新処理
            horizontalAim.Update(Time.deltaTime);
            verticalAim.Update(Time.deltaTime);
            
            //ダッシュの入力処理
            if (isDashHeld)
            {
                dashHoldTime += Time.deltaTime;
                if (dashHoldTime >= dashHoldThreshold)
                {
                    //移動入力があるときだけダッシュを開始
                    if (moveRawInput.sqrMagnitude > 0.01f)
                    {
                        BeginDash();
                    }
                }
            }
            
            //移動用ビットの更新処理
            UpdateBitFormation();
            
            //カーソル固定処理
            HandleCursorLock();
        }

        private void FixedUpdate()
        {
            //した2つは同じUpdateで呼ばないとカクつく
            //移動処理
            HandleMovement();
            
            //ダッシュ処理
            if (isDashing && currentDashEnergy > 1f)
            {
                // 移動入力がなければダッシュを中断する
                if (moveRawInput.sqrMagnitude < 0.01f)
                {
                    StopDash();
                }
                // 移動入力がある場合のみダッシュ処理を続ける
                else
                {
                    DoDash();
                    ConsumeDashEnergy();   
                }
            }else if (!isDashing)
            {
                RegenerateDashEnergy();
            }
            
            //キャラクターの向きをカメラに合わせる処理
            HandleCharacterRotation();

            if (controls.Player.Fire.IsPressed())
            {
                weapon.OnTriggerHold(emitPoint);
                weapon.SetLockOnTarget(lockOnManager.GetCurrentTarget());
            }
            
            //移動SE再生処理
            if (moveRawInput.sqrMagnitude > 0.01f || isThrustUp || isThrustDown)
            {
                if (!isEngineSoundPlaying)
                {
                    AudioManager.Instance.PlayLoopingSE(SoundType.EngineHum);
                    isEngineSoundPlaying = true;
                }
            }
            else
            {
                if (isEngineSoundPlaying)
                {
                    AudioManager.Instance.StopLoopingSE();
                    isEngineSoundPlaying = false;
                }
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
        
        
        /// <summary>
        /// プレイヤーの回転処理
        /// </summary>
        private void HandleCharacterRotation()
        {
            var horizontalRotation = Quaternion.AngleAxis(horizontalAim.Value, Vector3.up);
            var verticalRotation = Quaternion.AngleAxis(verticalAim.Value, Vector3.right);
            transform.rotation = horizontalRotation;
            camManager.EyeTransform.localRotation = verticalRotation;
        }
        

        /// <summary>
        /// 移動処理をまとめた関数
        /// </summary>
        private void HandleMovement()
        {
            // ----垂直スラスタ系----
            if (isThrustUp) rb.AddForce(Vector3.up * thrustAccel, ForceMode.Acceleration);
            if (isThrustDown) rb.AddForce(Vector3.down * thrustAccel, ForceMode.Acceleration);
            
            // ----水平移動----
            Vector3 moveDirection= transform.right * moveRawInput.x + transform.forward * moveRawInput.y;
            
            //目標速度計算
            Vector3 targetVel = new Vector3(moveDirection.x, 0, moveDirection.z)* moveSpeed;
            
            //加減速の判定
            Vector2 current2D = new Vector2(rb.velocity.x, rb.velocity.z);
            Vector2 target2D = new Vector2(targetVel.x, targetVel.z);
            bool isAccelerating = target2D.sqrMagnitude > current2D.sqrMagnitude;
            float smoothTime = isAccelerating ? accelTime : decelTime;
            
            //加減速に応じたsmoothTimeで加減速
            Vector3 smoothedHorizontalVel = Vector3.SmoothDamp(
                new Vector3(rb.velocity.x,0,rb.velocity.z),
                targetVel,
                ref currentVelocity,
                smoothTime
                );

            rb.velocity = new Vector3(smoothedHorizontalVel.x, rb.velocity.y, smoothedHorizontalVel.z);
        }
        
        
        /// <summary>
        /// ステップを試みる
        /// ステップ可能かを判断し、可能ならステップ処理を呼ぶ
        /// </summary>
        private void TryStep()
        {
            Debug.Log("TryStep");
            //ステップフラグが立っていて、入力があればダッシュをする
            if(!canStep|| moveRawInput.sqrMagnitude < 0.01f) return;
            DoStepAsync().Forget();
        }
        
        
        /// <summary>
        /// ステップ処理
        /// 入力方向にインパルスで力を加える
        /// クールダウンまで待つ非同期関数
        /// </summary>
        private async UniTaskVoid DoStepAsync()
        {
            canStep = false;
            
            //入力方向の単位ベクトルを取得
            Vector3 dir = (transform.right * moveRawInput.x + transform.forward * moveRawInput.y).normalized;
            if (dir.sqrMagnitude < 0.01f) dir = transform.forward;
            
            //インパルスを一度だけ加える
            rb.AddForce(dir * stepForce,ForceMode.Impulse);
            
            // TODO 無敵処理など
            //SE再生
            AudioManager.Instance.PlaySE(SoundType.Step);

            await UniTask.Delay((int)(stepCoolDown * 1000));

            canStep = true;
        }

        
        /// <summary>
        /// ダッシュボタン押下で呼び出される処理
        /// </summary>
        private void StartDashHold()
        {
            isDashHeld = true;
            dashHoldTime = 0f;
        }
        
        
        /// <summary>
        /// ダッシュボタン押上で呼び出される処理
        /// </summary>
        private void EndDashHold()
        {
            isDashHeld = false;
            dashHoldTime = 0f;
            StopDash();
        }

        /// <summary>
        /// ダッシュ開始処理
        /// </summary>
        private void BeginDash()
        {
            if (currentDashEnergy > 0f)
                isDashing = true;
        }
        
        /// <summary>
        /// ダッシュ終了処理 
        /// </summary>
        private void StopDash()
        {
            isDashing = false;
        }
        
        /// <summary>
        /// 入力方行に基づいてダッシュを行う
        /// </summary>
        private void DoDash()
        {
            Vector3 moveDirection = transform.right * moveRawInput.x + transform.forward * moveRawInput.y;
            Vector3 targetVel = new Vector3(moveDirection.x, 0, moveDirection.z) * dashSpeed;

            Vector2 current2D = new Vector2(rb.velocity.x, rb.velocity.z);
            Vector2 target2D = new Vector2(targetVel.x, targetVel.z);
            bool isAccelerating = target2D.sqrMagnitude > current2D.sqrMagnitude;
            float smoothTime = isAccelerating ? dashAccelTime : decelTime;
            
            //加減速に応じたsmoothTimeで加減速
            // Vector3 smoothedVel = Vector3.SmoothDamp(
            //     rb.velocity,
            //     targetVel,
            //     ref currentVelocity,
            //     smoothTime
            // );

            Vector3 smoothedVel = Vector3.MoveTowards(rb.velocity, targetVel, dashSpeed * Time.deltaTime / smoothTime);
            rb.velocity = smoothedVel;
        }
        
        
        /// <summary>
        /// ダッシュエネルギーの消費処理
        /// </summary>
        private void ConsumeDashEnergy()
        {
            //エネルギーをFixedUpdate時間で減らす
            currentDashEnergy -= dashEnergyConsume * Time.deltaTime;
            
            //エネルギーが0を下回らないようにする
            if (currentDashEnergy <= 0)
            {
                currentDashEnergy = 0;
                StopDash();
            }
            //todo エネルギーをUIに反映
            hudManager.SetBoostEnergyValue(currentDashEnergy);
        }
        
        /// <summary>
        /// ダッシュエネルギーを時間経過で回復させる処理
        /// </summary>
        private void RegenerateDashEnergy()
        {
            //最大値以上なら何もしない
            if(currentDashEnergy <  dashEnergyMax)
            { 
                //エネルギー回復処理
                currentDashEnergy += dashEnergyRegeneration * Time.deltaTime;
            
                //最大値クリップ
                currentDashEnergy = Mathf.Min(currentDashEnergy, dashEnergyMax);

                //todo エネルギーをUIに反映
                hudManager.SetBoostEnergyValue(currentDashEnergy);
            }
        }
        
        /// <summary>
        /// 現在の入力からビットのフォーメーション状態を決定し、BitFormationManagerに伝える
        /// </summary>
        private void UpdateBitFormation()
        {
            BitFormationState newState = DetermineBitState();

            if (newState != currentBitState)
            {
                currentBitState = newState;
                if (formationDictionary.TryGetValue(currentBitState, out BitFormationData data))
                {
                    bitManager.SetTargetFormation(data);
                }
            }
        }

        /// <summary>
        /// 現在の入力値から、どの状態であるべきかを判断する
        /// </summary>
        private BitFormationState DetermineBitState()
        {
            if (isThrustUp) return BitFormationState.Up;
            if (isThrustDown) return BitFormationState.Down;

            if (moveRawInput.sqrMagnitude < 0.01f)
            {
                return BitFormationState.Idle;
            }

            if (Mathf.Abs(moveRawInput.x) > Mathf.Abs(moveRawInput.y))
            {
                return moveRawInput.x > 0 ? BitFormationState.Right : BitFormationState.Left;
            }else
            {
                return moveRawInput.y > 0 ? BitFormationState.Forward : BitFormationState.Backward;
            }
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