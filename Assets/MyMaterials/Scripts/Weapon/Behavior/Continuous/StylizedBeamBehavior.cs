using UnityEngine;
using UnityEngine.VFX;


namespace weapon
{
    [CreateAssetMenu(menuName = "Weapons/Behavior/StylizedBeam")]
    public class StylizedBeamBehavior : WeaponBehavior
    {
        //todo 後々にScriptableObjectから参照にする
        [Header("Settings")]
        [SerializeField]private float damage = 10f;
        [SerializeField]private float shootRange = 50f;
        [SerializeField]private float shootInterval = 0.1f;
    
        [Header("Effects")]
        [SerializeField] private GameObject bulletLinePrefab;
        [SerializeField] private float bulletLineLifetime = 1.0f;
        [SerializeField] private Vector3 bulletLineOffset;
        
        [SerializeField] private GameObject muzzleFlashPrefab;
        [SerializeField] private float muzzleFlashLifetime = 0.5f;
        [SerializeField] private Vector3 muzzleFlashOffset;
        
        [SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] private Vector3 hitEffectOffset;
        
        private bool isFiring = false;
        private float lastFireTime = 0f;
        private GameObject hitEffect;
        private GameObject muzzleFlash;
        private GameObject bulletLine;
        
        private int layerMask01 = 1 << 11; //Layer10までを無視 
        public override void OnTriggerDown(Transform muzzle, Transform target)
        {
            //引き金を引いた瞬間に発射
            isFiring = true;
            Shoot(muzzle);
            lastFireTime = Time.time;
        }

        public override void OnTriggerHold(Transform muzzle, Transform target)
        {
            if(!isFiring) return;
            if (Time.time - lastFireTime >= shootInterval)
            {
                Shoot(muzzle);
                lastFireTime = Time.time;
            }
        }

        public override void OnTriggerUp(Transform muzzle, Transform target)
        {
            isFiring = false;
        }

        private void Shoot(Transform muzzle)
        {
            Ray ray = new Ray(muzzle.position, muzzle.forward);
            //Layer10までを無視
            if (Physics.Raycast(ray, out RaycastHit hit, shootRange, layerMask01))
            {
                Debug.Log(hit.transform.tag);
                
                //ヒットエフェクトの生成
                if (hitEffectPrefab)
                {
                    if (hitEffect)
                    {
                        hitEffect.transform.position = hit.point + hitEffectOffset;
                        hitEffect.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                        hitEffect.SetActive(true);
                    }
                    else
                    {
                        hitEffect = Instantiate(hitEffectPrefab, hit.point + hitEffectOffset, Quaternion.identity);
                    }
                }
            
                //ダメージ処理
                var damageble = hit.collider.GetComponent<IDamageble>();
                if (damageble != null)
                {
                    Vector3 hitPoint = hit.point;
                    Vector3 hitDirection = ray.direction;

                    damageble.TakeDamage(damage, hitPoint, hitDirection);
                }
            }
            
            SpawnMuzzleFlash(muzzle);
            SpawnBulletLineVFX(muzzle);
        }
        
        private void SpawnMuzzleFlash(Transform muzzle)
        {
            if(muzzleFlashPrefab == null) return;
            var go = Instantiate(
                muzzleFlashPrefab,
                muzzle.position + muzzleFlashOffset,
                muzzle.rotation,
                muzzle
            );
            Destroy(go,muzzleFlashLifetime);
        }
        
        private void SpawnBulletLineVFX(Transform muzzle)
        {
            if (bulletLinePrefab == null) return;
            // 毎回インスタンス化して確実に最初から再生
            var go = Instantiate(
                bulletLinePrefab,
                muzzle.position + bulletLineOffset,
                muzzle.rotation,
                muzzle
            );
            var vfx = go.GetComponent<VisualEffect>();
            if (vfx != null)
            {
                vfx.Stop();  // 念のため既存再生をリセット
                vfx.Play();  // 再生開始
            }
            // bulletLineLifetime 秒後にオブジェクトごと破棄
            Destroy(go, bulletLineLifetime);
        }
    }
}