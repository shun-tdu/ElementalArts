using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using Utility;
using Singletons;

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
        [SerializeField] private Color reticleLockOnColor;

        [Header("Lock-On")]
        [SerializeField] private float lockOnRange = 100f;  //ロックオン可能な最大距離
        [field: SerializeField] public LayerMask LockOnLayer { get; private set; }

        [Header("Lock-On Box UI")]
        [SerializeField] private Image lockOnBoxUI;
        [SerializeField] private float reticleTransitionDuration = 0.2f;
        [SerializeField] private Color lockOnBoxNormalColor;
        [SerializeField] private Color lockOnBoxRockOnColor;

        [Header("Aperture Setting")] 
        [SerializeField] private Image apertureGaugeLeft;
        [SerializeField] private Image apertureGaugeRight;
        [SerializeField] private Image aperturePartHorizon;
        [SerializeField] private Image aperturePartVertical;

        //ロックオン関連フィールド
        public Transform MainLockOnTarget { get; private set; }
        public float MainTargetDistance { get; private set; } 
        public bool IsLockedOn { get; private set; } = false;
        
        private Camera mainCamera;
        private PlayerController playerController;
        private DestructionNotifier currentNotifier;
        private Vector2 defaultPivotPosReticleImage;
        private Vector2 defaultPivotPosLockOnBoxUI;
        private LockOnState currentState = LockOnState.Free;
        private List<Transform> targetsInZone = new List<Transform>();
        private InputAction switchTargetAction;

        
        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            mainCamera = Camera.main;
            defaultPivotPosReticleImage = reticleImage.rectTransform.pivot;
            defaultPivotPosLockOnBoxUI = lockOnBoxUI.rectTransform.pivot;

            if (playerController.LockOnAction != null)
            {
                switchTargetAction = playerController.LockOnAction.actionMap.asset.FindAction("SwitchTarget");
            }
        }


        private void OnEnable()
        {
            if (switchTargetAction != null)
            {
                switchTargetAction.performed += OnSwitchTarget;
                switchTargetAction.Enable();
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
            if (currentState == LockOnState.Free && MainLockOnTarget)
            {
                ChangeLockOnState(LockOnState.Weak);
            }

            //WeakLock → Freeへの遷移
            if (currentState == LockOnState.Weak && !MainLockOnTarget || MainTargetDistance > lockOnRange)
            {
                ChangeLockOnState(LockOnState.Free);
            }

            //Intention → Freeへの遷移
            if ((currentState == LockOnState.Intention && playerController.LockOnAction.triggered) ||
                (currentState == LockOnState.Intention && !MainLockOnTarget)||
                (currentState == LockOnState.Intention && MainTargetDistance > lockOnRange))
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
        /// LockOnStateのEnter処理
        /// </summary>
        /// <param name="state">現在のLockOnState</param>
        void OnEnterLockOnState(LockOnState state)
        {
            switch (state)
            {
                case LockOnState.Free:
                    ClearLock();
                    
                    //----小レティクルと大レティクルの有効化と通常色への変更----//
                    reticleImage.gameObject.SetActive(true);
                    reticleImage.color = reticleNormalColor;
                    lockOnBoxUI.gameObject.SetActive(true);
                    lockOnBoxUI.color = lockOnBoxNormalColor;
                    
                    //----小レティクルを画面中央に戻す----//
                    MoveUIToCenterAsync(reticleImage, defaultPivotPosReticleImage, reticleTransitionDuration);
                    
                    //----大レティクルを画面中央に戻す----//
                    MoveUIToCenterAsync(lockOnBoxUI, defaultPivotPosLockOnBoxUI, reticleTransitionDuration);
                    
                    //----絞りゲージを初期化----//
                    SetAperture(1f);
                    
                    break;

                case LockOnState.Weak:
                    //----小レティクルのロックオン色への変更----//
                    reticleImage.color = reticleLockOnColor;
                    break;
                case LockOnState.Intention:
                    //----現在のロックオンを解除----//
                    ClearLock();
                    
                    //----画面の中心に最も近いものをMainTargetにする----//
                    // SearchForTargetInBox();
                    UpdateTargetsInZone();

                    Transform bestTarget = FindBestTargetInList();
                    if (bestTarget != null)
                    {
                        LockOnTo(bestTarget);
                    }
                    
                    //----大レティクル、小レティクルのロックオン色への変更----//
                    reticleImage.color = reticleLockOnColor;
                    lockOnBoxUI.color = lockOnBoxRockOnColor;
                    
                    //----SEの再生----//
                    AudioManager.Instance.PlaySE(SoundType.IntentionLockOn);
                    break;
            }
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
                    targetsInZone.Clear();
                    break;
                case LockOnState.Intention:
                    ClearLock();
                    break;
            }
        }
        

        /// <summary>
        /// フリーロックオン状態のUpdate処理
        /// </summary>
        private void HandleFree()
        {
            //レティクル領域内の敵を探索、敵がいればMainTargetに設定
            // SearchForTargetInBox();
            UpdateTargetsInZone();
            Transform bestTarget = FindBestTargetInList();
            if (bestTarget != null && !IsLockedOn)
            {
                LockOnTo(bestTarget);
            }
        }

        
        /// <summary>
        /// 弱ロックオン状態のUpdate処理
        /// </summary>
        private void HandleWeak()
        {
            //大レティクル領域内にメインターゲットがいるかを確認
            UpdateTargetsInZone();
            CheckIfTargetIsOutOfBox();
            
            //メインターゲットが存在しなければreturn
            if (!MainLockOnTarget) return;
            
            //小レティクルの移動処理
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(MainLockOnTarget.position);
            
            //絞りゲージのアニメーション
            SetAperture(1f - MainTargetDistance / lockOnRange);

            if (screenPosition.z > 0 &&
                screenPosition.x > 0 && screenPosition.x < Screen.width &&
                screenPosition.y > 0 && screenPosition.y < Screen.height)
            {
                reticleImage.gameObject.SetActive(true);
                lockOnBoxUI.gameObject.SetActive(true);
                reticleImage.transform.position = screenPosition;
                reticleImage.color = reticleLockOnColor;
            }
            else
            {
                reticleImage.gameObject.SetActive(false);
                lockOnBoxUI.gameObject.SetActive(false);
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
            
            //絞りゲージのアニメーション
            SetAperture(1f - MainTargetDistance / lockOnRange);
            
            if (screenPosition.z > 0 &&
                screenPosition.x > 0 && screenPosition.x < Screen.width &&
                screenPosition.y > 0 && screenPosition.y < Screen.height)
            {
                //----小レティクルの追従処理----//
                reticleImage.gameObject.SetActive(true);
                reticleImage.transform.position = screenPosition;   
                reticleImage.color = reticleLockOnColor;        
                
                //----大レティクルの追従処理----//
                lockOnBoxUI.gameObject.SetActive(true);
                lockOnBoxUI.transform.position = screenPosition;
            }
            else
            {
                reticleImage.gameObject.SetActive(false);
                lockOnBoxUI.gameObject.SetActive(false);
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
            
            AudioManager.Instance.PlaySE(SoundType.TargetAcquired);
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
        /// 指定されたUI要素を、時間をかけて画面中央へスムーズに移動させる
        /// </summary>
        /// <param name="img">移動させるImage</param>
        /// <param name="defaultPivotPos">初期状態のPivot位置</param>
        /// <param name="duration">移動にかける時間（秒）</param>
        private async UniTaskVoid MoveUIToCenterAsync(Image img, Vector2 defaultPivotPos, float duration)
        {
            //移動開始時の位置を取得
            Vector3 startPos = img.rectTransform.position;
            //目標値計算
            Vector3 endPos = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

            float elapsedTime = 0f;
            
            while (elapsedTime < duration)
            {
                //経過時間に基づいて位置を線形補間
                img.rectTransform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
                
                //時間を更新
                elapsedTime += Time.deltaTime;
                
                //次フレームまで待機
                await UniTask.Yield();
            }
            
            ResetUIToCenter(img, defaultPivotPos);
        }

        /// <summary>
        /// レティクルを初期化する
        /// 徐々に中心に戻す
        /// </summary>
        /// <param name="img">初期化するImage</param>
        private void ResetUIToCenter(Image img, Vector2 pivotPos)
        {
            RectTransform rt = img.rectTransform;
            rt.anchorMin = rt.anchorMax = rt.pivot = pivotPos;
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

    
        /// <summary>
        /// Apertureを指定した割合に即した表示にする
        /// 入力は表示割合
        /// </summary>
        private void SetAperture(float amount)
        {
            float fillAmount = Mathf.Clamp(amount, 0f, 1f);
            apertureGaugeLeft.fillAmount = Mathf.Clamp(amount, 0f, 1f);
            apertureGaugeRight.fillAmount = Mathf.Clamp(amount, 0f, 1f);

            aperturePartVertical.rectTransform.localEulerAngles = new Vector3(0f, 0f, (1 - amount) * 90f);
        }

        /// <summary>
        /// マウスホイールのスクロール入力を受け取った時の処理
        /// </summary>
        private void OnSwitchTarget(InputAction.CallbackContext context)
        {
            if(currentState != LockOnState.Weak||targetsInZone.Count <= 1) return;

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
            var potentialTargets = Physics.OverlapSphere(playerController.transform.position, lockOnRange, LockOnLayer);

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