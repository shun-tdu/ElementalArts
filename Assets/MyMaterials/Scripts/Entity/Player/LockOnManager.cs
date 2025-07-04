using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using MyMaterials.Scripts.Utility;
using MyMaterials.Scripts.Singletons;

namespace MyMaterials.Scripts.Entity.Player
{
    // public enum ReticleGauge
    // {
    //     Top,    // レティクル上ゲージ
    //     Left,   // レティクル左ゲージ
    //     Right  // レティクル右ゲージ
    // }
    
    
    /// <summary>
    /// Playerのロックオンステート
    /// Free:未ロックオン
    /// Weak:弱ロックオン 大レティクル内に入った敵をターゲットにする、大レティクルから外れたらロックオン解除
    /// </summary>
    public enum LockOnState { Free, Weak, Intention };

    public class LockOnManager : MonoBehaviour
    {
        [Header("Lock-On Logic")]
        [SerializeField] private RectTransform lockOnBoxRect; 
        [field : SerializeField] public float LockOnRange { get; private set; } = 100f;  //ロックオン可能な最大距離
        [field: SerializeField] public LayerMask LockOnLayer { get; private set; }
        
        // ----UI通知用のイベント定義----
        public event Action<LockOnState> OnStateChanged;
        public event Action<Transform> OnTargetAcquired;
        public event Action OnTargetLost;
        
        // ----公開プロパティ----
        public Transform MainLockOnTarget { get; private set; }
        public float MainTargetDistance { get; private set; }
        public bool IsLockedOn { get; private set; } = false;
        public LockOnState CurrentState { get; private set; } = LockOnState.Free;
        public IReadOnlyList<Transform> TargetsInZone => targetsInZone;

        // ----内部変数----
        private Camera mainCamera;
        private PlayerController playerController;
        private DestructionNotifier currentNotifier;
        private List<Transform> targetsInZone = new List<Transform>();
        private InputAction switchTargetAction;

        
        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            mainCamera = Camera.main;
        }

        private void Start()
        {
            if (playerController.LockOnAction != null)
            {
                switchTargetAction = playerController.LockOnAction.actionMap.asset.FindAction("SwitchTarget");
                
                if (switchTargetAction != null)
                {
                    switchTargetAction.performed += OnSwitchTarget;
                    switchTargetAction.Enable();
                }
            }
        }

        private void OnDisable()
        {
            if (switchTargetAction != null)
            {
                switchTargetAction.performed -= OnSwitchTarget;
                switchTargetAction.Disable();
            }
        }


        private void Update()
        {
            // メインターゲットとの距離を計算
            if (MainLockOnTarget != null)
            {
                MainTargetDistance =
                    Vector3.Distance(MainLockOnTarget.position, playerController.transform.position);
            }
            
            //Free → WeakLockへの遷移
            if (CurrentState == LockOnState.Free && MainLockOnTarget)
            {
                ChangeLockOnState(LockOnState.Weak);
            }

            //WeakLock → Freeへの遷移
            if (CurrentState == LockOnState.Weak && !MainLockOnTarget || MainTargetDistance > LockOnRange)
            {
                ChangeLockOnState(LockOnState.Free);
            }

            //Intention → Freeへの遷移
            if ((CurrentState == LockOnState.Intention && playerController.LockOnAction.triggered) ||
                (CurrentState == LockOnState.Intention && !MainLockOnTarget)||
                (CurrentState == LockOnState.Intention && MainTargetDistance > LockOnRange))
            {
                ChangeLockOnState(LockOnState.Free);
            }
            
            //WeakLock → Intentionへの遷移
            if (CurrentState == LockOnState.Weak && playerController.LockOnAction.triggered)
            {
                ChangeLockOnState(LockOnState.Intention);
            }


            //ステート毎の毎フレーム処理
            switch (CurrentState)
            {
                case LockOnState.Free:
                    UpdateTargetsInZone();
                    Transform bestTarget = FindBestTargetInList();
                    if(bestTarget !=null && !IsLockedOn) LockOnTo(bestTarget);
                    break;
                case LockOnState.Weak:
                    UpdateTargetsInZone();
                    CheckIfTargetIsOutOfBox();
                    break;
                case LockOnState.Intention:
                    // プレイヤーが倒されたり、距離が離れたりするチェックはUpdateの遷移判定で行う
                    break;
            }
        }
        
        
        /// <summary>
        /// LockOnStateを遷移させる関数
        /// Exit,Enter処理をしているので、必ずこの関数経由で状態を遷移させること
        /// </summary>
        /// <param name="nextState"></param>
        void ChangeLockOnState(LockOnState nextState)
        {
            OnExitLockOnState(CurrentState);
            CurrentState = nextState;
            OnEnterLockOnState(nextState);
            
            // HUDManagerに状態変化を通知
            OnStateChanged?.Invoke(nextState);
        }

        
        /// <summary>
        /// LockOnStateのEnter処理
        /// </summary>
        /// <param name="state">現在のLockOnState</param>
        void OnEnterLockOnState(LockOnState state)
        {
            if (state == LockOnState.Intention)
            {
                ClearLock();
                UpdateTargetsInZone();
                Transform bestTarget = FindBestTargetInList();
                if(bestTarget != null) LockOnTo(bestTarget);
            }
            else if (state == LockOnState.Free)
            {
                ClearLock();    
            }
        }
        
        
        /// <summary>
        /// LockOnStateのExit処理
        /// </summary>
        /// <param name="state">現在のLockOnState</param>
        void OnExitLockOnState(LockOnState state)
        {
            if(state == LockOnState.Weak) targetsInZone.Clear();
            else if (state == LockOnState.Intention) ClearLock();
        }
        
        
        /// <summary>
        /// 引数のtargetに対してロックオン処理
        /// 破壊時のイベント購読、PlayerControllerクラスへのターゲット指定を行う
        /// </summary>
        /// <param name="target"></param>
        private void LockOnTo(Transform target)
        {
            if (IsLockedOn) return;

            MainLockOnTarget = target;
            IsLockedOn = true;

            currentNotifier = MainLockOnTarget.GetComponent<DestructionNotifier>() ??
                              MainLockOnTarget.gameObject.AddComponent<DestructionNotifier>();
            currentNotifier.OnDestroyed += OnTargetDestroyed;
            playerController.SetTarget(MainLockOnTarget);
            
            //UIにターゲット補足を通知
            OnTargetAcquired?.Invoke(target);
            
            //ロックオンサウンド再生
            AudioManager.Instance.PlaySE(SoundType.TargetAcquired);
        }


                
        /// <summary>
        /// ロックオン解除処理
        /// </summary>
        private void ClearLock()
        {
            if(!IsLockedOn) return;
            IsLockedOn = false;
            if (currentNotifier != null) currentNotifier.OnDestroyed -= OnTargetDestroyed;
            currentNotifier = null;
            MainLockOnTarget = null;
            playerController.SetTarget(null);
            
            // UIにターゲット喪失を通知
            OnTargetLost?.Invoke();
        }
        

        /// <summary>
        /// ターゲットが大レティクル外にいるかを確認
        /// 外側にいる場合はロックオン解除
        /// </summary>
        private void CheckIfTargetIsOutOfBox()
        {
            //MainLockOnTargetが消えるか、非アクティブのときもロックオン解除
            if (MainLockOnTarget == null || !MainLockOnTarget.gameObject.activeInHierarchy)
            {
                ClearLock();
                return;
            }
            
            //ターゲットが大レティクル外にいる場合もロックオン解除
            if (!IsTargetInLockOnZone(MainLockOnTarget))
            {
                ClearLock();
            }
        }


        /// <summary>
        /// ターゲットがレティクル領域内に存在するか確認
        /// </summary>
        private bool IsTargetInLockOnZone(Transform target)
        {
            if (lockOnBoxRect == null || mainCamera == null) return false;
            //ワールド座標をスクリーン座標に変換
            Vector3 screenPoint = mainCamera.WorldToScreenPoint(target.position);
            if (screenPoint.z <= 0) return false;

            // レティクルの中心座標を取得
            Vector2 zoneCenter = lockOnBoxRect.position;

            // 中心からターゲットのスクリーン座標までの距離を計算
            float lockOnRadius = lockOnBoxRect.rect.width / 2f;
            float distanceToTarget = Vector2.Distance(zoneCenter, screenPoint);

            // 距離が半径の内側にあればtrue、外側にあればfalseを返す
            return distanceToTarget <= lockOnRadius;
        }
        
        
        /// <summary>
        /// 現在ロックオン中のターゲットを返す
        /// </summary>
        public Transform GetCurrentTarget()
        {
            return MainLockOnTarget;
        }

        
        //ターゲット破壊時のコールバック
        private void OnTargetDestroyed()
        {
            ClearLock();
        }
        
        
        /// <summary>
        /// マウスホイールのスクロール入力を受け取った時の処理
        /// </summary>
        private void OnSwitchTarget(InputAction.CallbackContext context)
        {
            if(CurrentState != LockOnState.Weak||targetsInZone.Count <= 1) return;

            float scrollValue = context.ReadValue<Vector2>().y;

            if (scrollValue > 0.1f)
            {
                CycleTarget(1);
            }else if (scrollValue < -0.1f)
            {
                CycleTarget(-1);
            }
        }

        
        /// <summary>
        /// ターゲットリストを循環して次のターゲットに切り替える
        /// </summary>
        private void CycleTarget(int direction)
        {
            if(MainLockOnTarget == null) return;
            
            int currentIndex = targetsInZone.IndexOf(MainLockOnTarget);
            if(currentIndex == -1) return;

            int nextIndex = (currentIndex + direction + targetsInZone.Count) % targetsInZone.Count;
            
            ClearLock();
            LockOnTo(targetsInZone[nextIndex]);
        }

        
        /// <summary>
        /// ロックオンボックス内にいるターゲットのリストを更新する
        /// </summary>
        private void UpdateTargetsInZone()
        {
            targetsInZone.Clear();
            var potentialTargets = Physics.OverlapSphere(playerController.transform.position, LockOnRange, LockOnLayer);

            foreach (var targetColider in potentialTargets)
            {
                if (IsTargetInLockOnZone(targetColider.transform))
                {
                    targetsInZone.Add(targetColider.transform);
                }
            }
        }

        
        /// <summary>
        /// ターゲットリストの中から画面中央に最も近いものを見つける
        /// </summary>
        private Transform FindBestTargetInList()
        {
            Transform bestTarget = null;
            float minScreenDistance = float.MaxValue;
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

            foreach (var target in targetsInZone)
            {
                Vector3 screenPoint = mainCamera.WorldToScreenPoint(target.position);
                if (screenPoint.z > 0)
                {
                    float distance = Vector2.Distance(screenPoint, screenCenter);
                    if (distance < minScreenDistance)
                    {
                        minScreenDistance = distance;
                        bestTarget = target;
                    }
                }
            }

            return bestTarget;
        }
    }
}