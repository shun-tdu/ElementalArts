using System;
using UnityEngine;
using UnityEngine.UI;
using Utility;


namespace Player
{
    /// <summary>
    /// Playerのロックオンステート
    /// Free:未ロックオン
    /// Weak:弱ロックオン 大レティクル内に入った敵をターゲットにする、大レティクルから外れたらロックオン解除
    /// </summary>
    public enum LockOnState
    {
        Free,
        Weak,
        Intention
    };

    public class LockOnManager : MonoBehaviour
    {
        [Header("Aiming Reticle UI")] 
        [SerializeField] private Image reticleImage;
        [SerializeField] private Color reticleNormalColor;
        [SerializeField] private Color reticleMainLockOnColor = Color.red;

        [Header("Lock-On")]
        [SerializeField] private float lockOnRange = 100f;  //ロックオン可能な最大距離
        [field: SerializeField] public LayerMask LockOnLayer { get; private set; }

        [Header("Lock-On Box UI")]
        [SerializeField] private Image lockOnBoxUI;

        //ロックオン関連フィールド
        public Transform MainLockOnTarget { get; private set; }
        public Transform SubLockOnTarget { get; private set; }
        public bool IsLockedOn { get; private set; } = false;
        
        private Camera mainCamera;
        private PlayerController playerController;
        private DestructionNotifier currentNotifier;

        private LockOnState currentState = LockOnState.Free;

        
        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            mainCamera = Camera.main;
        }
        

        private void Update()
        {
            //Free → WeakLockへの遷移
            if (currentState == LockOnState.Free && MainLockOnTarget)
            {
                ChangeLockOnState(LockOnState.Weak);
            }

            //WeakLock → Freeへの遷移
            if (currentState == LockOnState.Weak && !MainLockOnTarget)
            {
                ChangeLockOnState(LockOnState.Free);
            }

            //Intention → Freeへの遷移
            if ((currentState == LockOnState.Intention && playerController.LockOnAction.triggered) ||
                (currentState == LockOnState.Intention && !MainLockOnTarget))
            {
                ChangeLockOnState(LockOnState.Free);
            }
            
            //WeakLock → Intentionへの遷移
            if (currentState == LockOnState.Weak && playerController.LockOnAction.triggered)
            {
                ChangeLockOnState(LockOnState.Intention);
            }


            //ステート毎の毎フレーム処理
            switch (currentState)
            {
                case LockOnState.Free:
                    HandleFree();
                    break;
                case LockOnState.Weak:
                    //レティクル領域内にメインターゲットが居るか監視、領域外に出たらMainTargetをクリア
                    HandleWeak();
                    break;
                case LockOnState.Intention:
                    //大きいレティクルをTargetに追従させる
                    HandleIntention();
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
            OnExitLockOnState(currentState);
            currentState = nextState;
            OnEnterLockOnState(nextState);
        }

        
        /// <summary>
        /// LockOnStateのExit処理
        /// </summary>
        /// <param name="state">現在のLockOnState</param>
        void OnExitLockOnState(LockOnState state)
        {
            switch (state)
            {
                case LockOnState.Free:
                    break;
                case LockOnState.Weak:
                    break;
                case LockOnState.Intention:
                    break;
            }
        }

        
        /// <summary>
        /// LockOnStateのEnter処理
        /// </summary>
        /// <param name="state">現在のLockOnState</param>
        void OnEnterLockOnState(LockOnState state)
        {
            switch (state)
            {
                case LockOnState.Free:
                    //----小レティクルと大レティクルの有効化と通常色への変更----//
                    reticleImage.enabled = true;
                    reticleImage.color = reticleNormalColor;

                    lockOnBoxUI.enabled = true;
                    
                    //----小レティクルを画面中央に戻す----/
                    // RectTransform reticleImageRectTransform = reticleImage.rectTransform;
                    // reticleImageRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    // reticleImageRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    // reticleImageRectTransform.pivot = new Vector2(0.5f, 0.5f);
                    // reticleImageRectTransform.anchoredPosition = Vector2.zero;
                    ResetUIToCenter(reticleImage);
                    
                    //----大レティクルを画面中央に戻す----/
                    // RectTransform lockOnBoxUIRectTransform = lockOnBoxUI.rectTransform;
                    // lockOnBoxUIRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    // lockOnBoxUIRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    // lockOnBoxUIRectTransform.pivot = new Vector2(0.5f, 0.5f);
                    // lockOnBoxUIRectTransform.anchoredPosition = Vector2.zero;
                    ResetUIToCenter(lockOnBoxUI);
                    break;

                case LockOnState.Weak:
                    //Todo 小レティクルと大レティクルの有効化
                    break;
                case LockOnState.Intention:
                    //小レティクルと大レティクルの有効化
                    //MainTargetの再計算
                    //画面の中心に最も近いものをMainTargetにする
                    SearchForTargetInBox();
                    
                    break;
            }
        }


        /// <summary>
        /// フリーロックオン状態のUpdate処理
        /// </summary>
        private void HandleFree()
        {
            //レティクル領域内の敵を探索、敵がいればMainTargetに設定
            SearchForTargetInBox();
        }

        
        /// <summary>
        /// 弱ロックオン状態のUpdate処理
        /// </summary>
        private void HandleWeak()
        {
            //大レティクル領域内にメインターゲットがいるかを確認
            CheckIfTargetIsOutOfBox();

            if (!MainLockOnTarget) return;

            //小レティクルの移動処理
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(MainLockOnTarget.position);

            if (screenPosition.z > 0 &&
                screenPosition.x > 0 && screenPosition.x < Screen.width &&
                screenPosition.y > 0 && screenPosition.y < Screen.height)
            {
                reticleImage.enabled = true;                        //小レティクルの有効化
                reticleImage.transform.position = screenPosition;   // ターゲットに追従
                reticleImage.color = reticleMainLockOnColor;        // メインロックオン用の色に
            }
            else
            {
                reticleImage.enabled = false;                       //レティクルの無効化
            }
        }

        
        /// <summary>
        /// インテンションロックオン時のUpdate処理
        /// </summary>
        private void HandleIntention()
        {
            //小レティクルと大レティクルをターゲットに追従
            if(!MainLockOnTarget) return;
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(MainLockOnTarget.position);
            
            if (screenPosition.z > 0 &&
                screenPosition.x > 0 && screenPosition.x < Screen.width &&
                screenPosition.y > 0 && screenPosition.y < Screen.height)
            {
                //----小レティクルの追従処理----//
                reticleImage.enabled = true;                        //小レティクルの有効化
                reticleImage.transform.position = screenPosition;   // ターゲットに追従
                reticleImage.color = reticleMainLockOnColor;        // メインロックオン用の色に
                
                //----大レティクルの追従処理----//
                lockOnBoxUI.enabled = true;
                lockOnBoxUI.transform.position = screenPosition;
            }
            else
            {
                reticleImage.enabled = false;                       //レティクルの無効化
                lockOnBoxUI.enabled = false;
            }
        }
        

        /// <summary>
        /// レティクル領域内の最も近いターゲットを取得
        /// ロックオン処理を行う
        /// </summary>
        private void SearchForTargetInBox()
        {
            var potentialTargets = Physics.OverlapSphere(transform.position, lockOnRange, LockOnLayer);

            Transform bestTarget = null;
            float minScreenDistace = float.MaxValue;
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

            foreach (var targetCollider in potentialTargets)
            {
                if (IsTargetInLockOnZone(targetCollider.transform))
                {
                    Vector3 screenPoint = mainCamera.WorldToScreenPoint(targetCollider.transform.position);
                    float distance = Vector2.Distance(screenPoint, screenCenter);
                    if (distance < minScreenDistace)
                    {
                        minScreenDistace = distance;
                        bestTarget = targetCollider.transform;
                    }
                }
            }

            if (bestTarget != null)
            {
                LockOnTo(bestTarget);
            }
        }


        private void CheckIfTargetIsOutOfBox()
        {
            if (MainLockOnTarget == null || !MainLockOnTarget.gameObject.activeInHierarchy)
            {
                ClearLock();
                return;
            }

            if (!IsTargetInLockOnZone(MainLockOnTarget))
            {
                ClearLock();
            }
        }


        /// <summary>
        /// ターゲットがレティクル領域内に存在するか確認
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private bool IsTargetInLockOnZone(Transform target)
        {
            if (lockOnBoxUI == null || mainCamera == null) return false;
            //ワールド座標をスクリーン座標に変換
            Vector3 screenPoint = mainCamera.WorldToScreenPoint(target.position);
            if (screenPoint.z <= 0) return false;

            // レティクルの中心座標を取得
            Vector2 zoneCenter = lockOnBoxUI.rectTransform.position;

            // 中心からターゲットのスクリーン座標までの距離を計算
            float lockOnRadius = lockOnBoxUI.rectTransform.rect.width / 2f;
            float distanceToTarget = Vector2.Distance(zoneCenter, screenPoint);

            // 距離が半径の内側にあればtrue、外側にあればfalseを返す
            return distanceToTarget <= lockOnRadius;
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
        /// レティクルを初期化する
        /// </summary>
        /// <param name="img">初期化するImage</param>
        private void ResetUIToCenter(Image img)
        {
            RectTransform rt = img.rectTransform;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
        }
        
        /// <summary>
        /// ロックオン解除処理
        /// </summary>
        private void ClearLock()
        {
            IsLockedOn = false;
            if (currentNotifier != null)
                currentNotifier.OnDestroyed -= OnTargetDestroyed;

            currentNotifier = null;
            MainLockOnTarget = null;
            playerController.SetTarget(null);
        }
    }
}