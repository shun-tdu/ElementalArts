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
        
        //ロックオン関連フィールド
        [field:SerializeField]public LayerMask LockOnLayer { get; private set; } 
        public Transform MainLockOnTarget { get; private set; }
        public Transform SubLockOnTarget { get; private set; }
        public bool IsLockedOn { get; private set; } = false;
        public bool IsAimingSubTarget { get; private set; } = false;

        private CameraManager camManager;
        private PlayerController playerController;
        private DestructionNotifier currentNotifier;

        private void Awake()
        {
            camManager = GetComponent<CameraManager>();
            playerController = GetComponent<PlayerController>();
        }

        /// <summary>
        /// ロックオン処理
        /// 引数のTransformを起点に球状の領域を発生
        /// 領域内のオブジェクト内で視野範囲かつ最も近い敵をMainLockOnTargetに代入
        /// </summary>
        /// <param name="playerTransform"></param>
        public void ToggleLockOn(Transform playerTransform)
        {
            if (IsLockedOn)
            {
                // IsLockedOn = false;
                // MainLockOnTarget = null;
                // playerController.SetTarget(null);
                ClearLock();
            }
            else
            {
                FindBestTarget(playerTransform);
                if (MainLockOnTarget != null)
                {
                    //通知コンポーネントを取得
                    currentNotifier = MainLockOnTarget.GetComponent<DestructionNotifier>() ??
                                      MainLockOnTarget.gameObject.AddComponent<DestructionNotifier>();
                    currentNotifier.OnDestroyed += HandleTargetDestroyed;
                    IsLockedOn = true;
                    playerController.SetTarget(MainLockOnTarget);
                }
            }
        }
        
        /// <summary>
        /// 現在ロックオン中のターゲットを返す
        /// </summary>
        public Transform GetCurrentTarget()
        {
            return IsAimingSubTarget ? SubLockOnTarget : MainLockOnTarget;
        }


        /// <summary>
        /// 指定したTransformから球状の領域を発生
        /// 領域内の敵をリストアップ
        /// 最も近い敵をmainLockOnTargetに設定
        /// </summary>
        private void FindBestTarget(Transform transformFrom)
        {
            //シーン内のすべての敵候補を検索
            Collider[] potentialTargets = Physics.OverlapSphere(transformFrom.position, lockOnRange, LockOnLayer);

            Transform bestTarget = null;
            float minScreenDistance = float.MaxValue;

            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

            foreach (var targetCollider in potentialTargets)
            {
                // Vector3 directionToTarget = targetCollider.transform.position - cameraPivot.position;
                Vector3 directionToTarget = targetCollider.transform.position - camManager.EyeTransform.position;

                if (Vector3.Angle(camManager.EyeTransform.forward, directionToTarget) < lockOnAngle)
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

            MainLockOnTarget = bestTarget;
        }

        private void HandleTargetDestroyed()
        {
            ClearLock();
        }
        
        private void ClearLock()
        {
            IsLockedOn = false;
            if (currentNotifier != null)
                currentNotifier.OnDestroyed -= HandleTargetDestroyed;
            
            currentNotifier = null;
            MainLockOnTarget = null;
            playerController.SetTarget(null);
        }
        
        private void StartAimingSubTarget()
        {
            IsAimingSubTarget = true;
            if (MainLockOnTarget != null)
                SubLockOnTarget = MainLockOnTarget;
        }

        private void StopAimingSubTarget()
        {
            IsAimingSubTarget = false;
        }
    }
}