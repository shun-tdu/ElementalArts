using System;
using UnityEngine;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Controls
{
    public class EnemyVision : MonoBehaviour
    {
        [Header("視野の設定")] 
        [SerializeField, Range(0, 360)] private float viewAngle = 90f;
        [SerializeField] private float viewRadius = 40f;
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private LayerMask obstacleMask;

        /// <summary>
        /// 視野範囲内にプレイヤーがいるかを確認する
        /// </summary>
        /// <returns>発見した場合はプレイヤーのTransform、見つからなければnull</returns>
        public Transform FindPlayerInView()
        {
            Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

            foreach (var targetCollider in targetInViewRadius)
            {
                Transform target = targetCollider.transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    float distToTarget = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                    {
                        return target;
                    }
                }
            }
            return null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, viewRadius);

            Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
            Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
            Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);
        }

        private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.y;
            }

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }
}