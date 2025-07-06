using System;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

using MyMaterials.Scripts.Weapon;
using MyMaterials.Scripts.UI;
using MyMaterials.Scripts.Entity.Player.Bit;
using MyMaterials.Scripts.Managers.Singletons;

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
        [SerializeField] private float stepForce = 15f;     // ダッシュのインパルス強度
        [SerializeField] private float stepEnergyCost = 25f;
        [SerializeField] private float stepCoolDown = 1.0f; // ダッシュのクールダウン

        [Header("Dash Settings")] 
        [SerializeField] private float dashSpeed = 80f;
        [field:SerializeField] public float EnergyMax { get; private set; } = 100f;
        [SerializeField] private float dashEnergyConsume = 10f;
        [SerializeField] private float energyRegeneration = 20f;
        [SerializeField] private float dashHoldThreshold = 0.2f;
        [SerializeField] private float dashAccelTime = 1.0f;
        private float currentEnergy;
        private float dashHoldTime = 0f;
        private bool isDashing = false;
        private bool isDashHeld = false;
        
        [Header("Look")] 
        [SerializeField] public float lookSensitivity = 2f;
        
        [Header("武装 (Armaments)")] 
        [Tooltip("装備する武器スロットのリスト。最大4つまでを想定")]
        [SerializeField] private List<WeaponSystem> weaponSlots;    //複数の武器スロットを管理するリスト
        private int mainWeaponIndex = 0;
        private int subWeaponIndex = 1;     
        private bool isSet1Active = true;                                   //現在セット１(0, 1)がアクティブか
        
        [Header("Input Control(Lock Axes)")] 
        public AxisState horizontalAim;
        public AxisState verticalAim;

        [Header("ビット制御 (Bit Control)")]
        [SerializeField] private BitFormationManager bitManager;            // BitFormationManagerへの参照
        [SerializeField] private List<FormationMapping> formationMappings;  // 状態と設計図のマッピングリスト

        public event Action<WeaponSystem, WeaponSystem> OnActiveWeaponChanged;  // アクティブな武器が変更されたときに通知するイベント
        
        /* ========内部状態変数======== */
        // 操作系入力値
        private Vector2 moveRawInput = Vector2.zero;
        private bool isThrustUp = false;
        private bool isThrustDown = false;
        private bool isFireSecondaryWeapon = false;
        private bool canStep = true;
        private Vector3 currentVelocity;
        
        // ビット制御系
        private Dictionary<BitFormationState, BitFormationData> formationDictionary;
        private BitFormationState currentBitState;

        // SE制御系
        private bool isEngineSoundPlaying = false;
        
        // アタッチされているコンポーネントの内部参照
        private PlayerControls controls;
        private CameraManager camManager;
        private LockOnManager lockOnManager;
        private HUDManager hudManager;
        private Rigidbody rb;
        private Transform emitPoint;
        
        // ロックオン状態の外部参照
        public InputAction LockOnAction => controls.Player.LockOn;
        

        private void Awake()
        {
            // コンポーネントの初期化
            rb = GetComponent<Rigidbody>();
            emitPoint = transform.Find("EmitPoint");
            controls = new PlayerControls();
            camManager = GetComponent<CameraManager>();
            lockOnManager = GetComponent<LockOnManager>();
            hudManager = GameObject.Find("Canvas/InGameHUD").GetComponent<HUDManager>();

            formationDictionary = new Dictionary<BitFormationState, BitFormationData>();
            foreach (var mapping in formationMappings)
            {
                formationDictionary[mapping.state] = mapping.formationData;
            }
            
            // イベントの購読
            // ----Movement----
            controls.Player.Move.performed += ctx => moveRawInput = ctx.ReadValue<Vector2>();
            controls.Player.Move.canceled += ctx => moveRawInput = Vector2.zero;

            // ----Thrust----
            controls.Player.ThrustUp.performed += ctx => isThrustUp = true;
            controls.Player.ThrustUp.canceled += ctx => isThrustUp = false;
            controls.Player.ThrustDown.performed += ctx => isThrustDown = true;
            controls.Player.ThrustDown.canceled += ctx => isThrustDown = false;
            
            // ----Step----
            controls.Player.Step.performed += ctx => TryStep();
            
            // ----Dash----
            controls.Player.Dash.performed += _ => StartDashHold();
            controls.Player.Dash.canceled += _ => EndDashHold();
            
            // ----Switch Weapon----
            controls.Player.SwitchWeaponSet.performed += _ => SwitchActiveWeaponSet();
            
            // ----MainWeapon----
            controls.Player.Fire.started += _ => FireWeapon(GetMainWeapon());
            controls.Player.Fire.canceled += _ => GetMainWeapon()?.OnTriggerUp(emitPoint);
            
            // ---- SubWeapon----
            controls.Player.SeondaryFire.started += _ => FireWeapon(GetSubWeapon());
            controls.Player.SeondaryFire.canceled += _ => GetSubWeapon()?.OnTriggerUp(emitPoint);
            
            // ----Reload----
            controls.Player.Reload.performed += _ => ReloadActiveWeapon();
            
        }

        private void Start()
        {
            // カーソルの画面固定処理
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            // エナジーゲージの初期化
            currentEnergy = EnergyMax;
            
            // 最初の武器を有効化
            SetActiveWeapons(0,1);
        }

        private void OnEnable() => controls.Player.Enable();
        private void OnDisable() => controls.Player.Disable();

        private void Update()
        {
            // カメラステートの更新処理
            camManager.UpdateCameraState(transform, rb.velocity);
            
            // AxisStateの更新処理
            horizontalAim.Update(Time.deltaTime);
            verticalAim.Update(Time.deltaTime);
            
            // ダッシュの入力処理
            if (isDashHeld)
            {
                dashHoldTime += Time.deltaTime;
                if (dashHoldTime >= dashHoldThreshold)
                {
                    // 移動入力があるときだけダッシュを開始
                    if (moveRawInput.sqrMagnitude > 0.01f)
                    {
                        BeginDash();
                    }
                }
            }
            
            // 移動用ビットの更新処理
            UpdateBitFormation();
            
            // カーソル固定処理
            HandleCursorLock();
        }

        private void FixedUpdate()
        {
            // HandleMovement, HandleCharacterRotationは同じUpdateで呼ばないとカクつく
            // 移動処理
            HandleMovement();
            
            // ダッシュ処理
            if (isDashing && currentEnergy > 1f)
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
            
            // キャラクターの向きをカメラに合わせる処理
            HandleCharacterRotation();

            if (controls.Player.Fire.IsPressed())
            {
                HandleWeaponHold(GetMainWeapon());
            }

            if (controls.Player.SeondaryFire.IsPressed())
            {
                HandleWeaponHold(GetSubWeapon());
            }
            
            // 移動SE再生処理
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
        public void SetTarget(Transform target)
        {
            var mainWeapon = GetMainWeapon();
            if (mainWeapon != null)
            {
                mainWeapon.SetLockOnTarget(target);
            }
            var subWeapon = GetSubWeapon();
            if (subWeapon != null)
            {
                subWeapon.SetLockOnTarget(target);
            }
        }

        
        /// <summary>
        /// 現在のメイン武器を取得
        /// </summary>
        public WeaponSystem GetMainWeapon()
        {
            if (weaponSlots != null && weaponSlots.Count > mainWeaponIndex)
                return weaponSlots[mainWeaponIndex];
            return null;
        }
        
        
        /// <summary>
        /// 現在のメイン武器を取得
        /// </summary>
        public WeaponSystem GetSubWeapon()
        {
            if (weaponSlots != null && weaponSlots.Count > subWeaponIndex)
                return weaponSlots[subWeaponIndex];
            return null;
        }
        
        
        /// <summary>
        /// アクティブな武器を切り替える
        /// </summary>
        private void SwitchActiveWeaponSet()
        {
            // 装備が2つ以下なら切り替えない
            if(weaponSlots.Count <= 2) return;

            isSet1Active = !isSet1Active;

            if (isSet1Active)
            {
                SetActiveWeapons(0, 1);
            }
            else
            {
                // 4つない場合はサブはメインと同じにする
                int nextSubIndex = weaponSlots.Count > 3 ? 3 : 2;
                SetActiveWeapons(2, nextSubIndex);
            }
        }
        
        
        /// <summary>
        /// 現在アクティブな武器のWeaponSystemコンポーネントを取得する
        /// </summary>
        private void SetActiveWeapons(int mainIndex, int subIndex)
        {
            mainWeaponIndex = mainIndex;
            subWeaponIndex = subIndex;
            
            // Debug.Log($"武器セットを切り替え: Main={GetMainWeapon()?.name}, Sub={GetSubWeapon()?.name}");

            OnActiveWeaponChanged?.Invoke(GetMainWeapon(), GetSubWeapon());

            // TODO: HUDManagerに現在のアクティブ武器を通知する
            // todo hudManager.UpdateActiveWeapons(GetMainWeapon(), GetSubWeapon());
        }
        
        
        /// <summary>
        /// 指定された武器で攻撃を開始
        /// </summary>
        private void FireWeapon(WeaponSystem weapon)
        {
            if (weapon != null)
            {
                weapon.OnTriggerDown(emitPoint);
                weapon.SetLockOnTarget(lockOnManager.GetCurrentTarget());
            }
        }

        private void HandleWeaponHold(WeaponSystem weapon)
        {
            if (weapon != null)
            {
                weapon.OnTriggerHold(emitPoint);
                weapon.SetLockOnTarget(lockOnManager.GetCurrentTarget());
            }
        }
        
        /// <summary>
        /// 現在アクティブな武器をリロードを試みる
        /// </summary>
        private void ReloadActiveWeapon()
        {
            var mainWeapon = GetMainWeapon();
            if (mainWeapon != null)
            {
                mainWeapon.TryReload();
            }
            var subWeapon = GetSubWeapon();
            if (subWeapon != null)
            {
                subWeapon.TryReload();
            }
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
            // ステップフラグが立っていて、入力があればダッシュをする
            if(!canStep|| moveRawInput.sqrMagnitude < 0.01f) return;
            
            // エネルギーが足りているかチェック
            if (currentEnergy < stepEnergyCost)
            {
                AudioManager.Instance.PlaySE(SoundType.EnergyEmpty);
            }
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

            currentEnergy -= stepEnergyCost;
            hudManager.SetEnergyValue(currentEnergy);
            
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
            if (currentEnergy > 0f)
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

            Vector3 smoothedVel = Vector3.MoveTowards(rb.velocity, targetVel, dashSpeed * Time.deltaTime / smoothTime);
            rb.velocity = smoothedVel;
        }
        
        
        /// <summary>
        /// ダッシュエネルギーの消費処理
        /// </summary>
        private void ConsumeDashEnergy()
        {
            // エネルギーをFixedUpdate時間で減らす
            currentEnergy -= dashEnergyConsume * Time.deltaTime;
            
            // エネルギーが0を下回らないようにする
            if (currentEnergy <= 0)
            {
                currentEnergy = 0;
                StopDash();
            }
            
            // エネルギーをUIに反映
            hudManager.SetEnergyValue(currentEnergy);
        }
        
        
        /// <summary>
        /// ダッシュエネルギーを時間経過で回復させる処理
        /// </summary>
        private void RegenerateDashEnergy()
        {
            // 最大値以上なら何もしない
            if(currentEnergy <  EnergyMax)
            { 
                // エネルギー回復処理
                currentEnergy += energyRegeneration * Time.deltaTime;
            
                // 最大値クリップ
                currentEnergy = Mathf.Min(currentEnergy, EnergyMax);

                // エネルギーをUIに反映
                hudManager.SetEnergyValue(currentEnergy);
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