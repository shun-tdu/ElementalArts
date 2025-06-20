using System.Collections;
using UnityEngine;
using weapon.Behavior.Projectile.Impact;

namespace weapon.Behavior.Projectile.Core
{
    public class HomingProjectile : BaseProjectile
    {
        private Transform target;
        private float turnSpeed;

        [Header("ホーミング設定")] 
        [Tooltip("何秒後に追尾を開始するか")]
        [SerializeField] private float homingStartDelay = 0.5f;

        private bool isHoingActive = false;

        private void Start()
        {
            StartCoroutine(ActivateHomingCoroutine());
        }

        /// <summary>
        /// ホーミング弾としてImpact・ターゲット・回転速度を注入
        /// </summary>
        public void Initialize(IProjectileImpact impact, Transform homingTarget, float homingTurnSpeed)
        {
            base.Initialize(impact);
            target = homingTarget;
            turnSpeed = homingTurnSpeed;
        }

        private IEnumerator ActivateHomingCoroutine()
        {
            yield return new WaitForSeconds(homingStartDelay);
            isHoingActive = true;
        }
        
        private void FixedUpdate()
        {
            if (isHoingActive && target != null && Rb.velocity.sqrMagnitude > 0f)
            {
                //ターゲット方向
                Vector3 desireDir = (target.position - transform.position).normalized;
                //現在の速度方向
                Vector3 currentDir = base.Rb.velocity.normalized;
                //徐々にターゲット方向へ回転
                Vector3 newDir = Vector3.RotateTowards(
                    currentDir,
                    desireDir,
                    turnSpeed * Time.fixedDeltaTime,
                    0f
                );
                //速度ベクトルを更新
                base.Rb.velocity = newDir * base.Rb.velocity.magnitude;
                //見た向きも合わせる
                transform.rotation = Quaternion.LookRotation(newDir);
            }
        }
    }
}