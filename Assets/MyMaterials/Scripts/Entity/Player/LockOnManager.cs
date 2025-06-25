using System;
using UnityEngine;
using Utility;

namespace Player
{
    public class LockOnManager : MonoBehaviour
    {
        [Header("Lock-On")] 
        [SerializeField] private float lockOnRange = 100f;   //ロックオン可能な最大距離
        [SerializeField] private float lockOnAngle = 30f;    //画面中心からこの角度内を探索
        [field:SerializeField]public LayerMask LockOnLayer { get; private set; }

        [Header("Lock-On Box UI")] 
        [SerializeField] private RectTransform lockOnBoxUI;
        
        //ロックオン関連フィールド
        public Transform MainLockOnTarget { get; private set; }
        public Transform SubLockOnTarget { get; private set; }
        public bool IsLockedOn { get; private set; } = false;
        public bool IsAimingSubTarget { get; private set; } = false;

        private Camera mainCamera;
        private CameraManager camManager;
        private PlayerController playerController;
        private DestructionNotifier currentNotifier;

        private void Awake()
        {
            camManager = GetComponent<CameraManager>();
            playerController = GetComponent<PlayerController>();
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (IsLockedOn)
            {
                CheckIfTargetIsOutOfBox();
            }
            else
            {
                SearchForTargetInBox();
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
            Vector2 zoneCenter = lockOnBoxUI.position;
            
            // 中心からターゲットのスクリーン座標までの距離を計算
            float lockOnRadius = lockOnBoxUI.rect.width / 2f;
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
            if(IsLockedOn) return;

            MainLockOnTarget = target;
            IsLockedOn = true;

            currentNotifier = MainLockOnTarget.GetComponent<DestructionNotifier>() ??
                              MainLockOnTarget.gameObject.AddComponent<DestructionNotifier>();
            currentNotifier.OnDestroyed += OnTargetDestroyed;
            playerController.SetTarget(MainLockOnTarget);
        }

        /// <summary>
        /// ロックオン処理
        /// 引数のTransformを起点に球状の領域を発生
        /// 領域内のオブジェクト内で視野範囲かつ最も近い敵をMainLockOnTargetに代入
        /// </summary>
        /// <param name="playerTransform"></param>
        public void ToggleLockOn(Transform playerTransform)
        {
            // if (IsLockedOn)
            // {
            //     // IsLockedOn = false;
            //     // MainLockOnTarget = null;
            //     // playerController.SetTarget(null);
            //     ClearLock();
            // }
            // else
            // {
            //     FindBestTarget(playerTransform);
            //     if (MainLockOnTarget != null)
            //     {
            //         //通知コンポーネントを取得
            //         currentNotifier = MainLockOnTarget.GetComponent<DestructionNotifier>() ??
            //                           MainLockOnTarget.gameObject.AddComponent<DestructionNotifier>();
            //         currentNotifier.OnDestroyed += HandleTargetDestroyed;
            //         IsLockedOn = true;
            //         playerController.SetTarget(MainLockOnTarget);
            //     }
            // }
        }
        
        /// <summary>
        /// 現在ロックオン中のターゲットを返す
        /// </summary>
        public Transform GetCurrentTarget()
        {
            return IsAimingSubTarget ? SubLockOnTarget : MainLockOnTarget;
        }

        //ターゲット破壊時のコールバック
        private void OnTargetDestroyed()
        {
            ClearLock();
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