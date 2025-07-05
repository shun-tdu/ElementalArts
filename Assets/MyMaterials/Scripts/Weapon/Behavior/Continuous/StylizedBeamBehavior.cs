using UnityEngine;
using UnityEngine.VFX;
using MyMaterials.Scripts.Managers.Singletons;


namespace MyMaterials.Scripts.Weapon.Behavior.Continuous
{
    [CreateAssetMenu(menuName = "Weapons/Behavior/StylizedBeam")]
    public class StylizedBeamBehavior : WeaponBehavior, IChargeWeapon
    {
        //todo 後々にScriptableObjectから参照にする
        [Header("性能")]
        [SerializeField] private float damagePerSecond = 50f;
        [SerializeField] private float shootRange = 100f;
        [SerializeField] private float ammoConsumptionPerSecond = 20f; // 秒間エネルギー消費量
        [SerializeField] private float maxCharge = 100f;
    
        [Header("エフェクト(Effects)")]
        [SerializeField] private GameObject beamVFXPrefab;              // ビーム本体のVFXプレハブ
        [SerializeField] private EffectType muzzleFlashEffectType;      // マズルフラッシュのエフェクト
        [SerializeField] private EffectType hitEffectType;              // ヒットエフェクト
        
        public float MaxCharge => maxCharge;

        private static GameObject currentBeamInstance;
        private static Transform currentHitTarget;
        private static float lastHitTime;
        
        public override void OnTriggerDown(WeaponSystem owner, Transform muzzle, Transform target)
        {
            // 既にビームが出ていなければ、新しく生成
            if (currentBeamInstance == null && beamVFXPrefab!= null)
            {
                currentBeamInstance = Instantiate(beamVFXPrefab, muzzle.position, muzzle.rotation, muzzle);
            }
            // マズルフラッシュを再生
            EffectManager.Instance.PlayEffect(muzzleFlashEffectType, muzzle.position, muzzle.rotation);
        }

        public override void OnTriggerHold(WeaponSystem owner, Transform muzzle, Transform target)
        {   
            // ビームが出ていない or エネルギーがなければ処理をしない
            if (currentBeamInstance == null || owner.CurrentAmmo <= 0)
            {
                OnTriggerUp(owner, muzzle, target);
                return;
            }
            
            // エネルギーを継続的に消費
            owner.ConsumeAmmo(ammoConsumptionPerSecond * Time.deltaTime);
            
            // ビームの当たり判定
            Ray ray = new Ray(muzzle.position, muzzle.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, shootRange))
            {
                // ダメージを継続的に与える
                if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
                {
                    float damage = damagePerSecond * Time.deltaTime;
                    damageable.TakeDamage(damage,hit.point,ray.direction);
                }
                
                // ヒットエフェクトを再生
                // 連続で再生しないように、最後にヒットした相手と少し時間が経っているかをチェック
                if (currentHitTarget != hit.transform || Time.time > lastHitTime + 0.1f)
                {
                    EffectManager.Instance.PlayEffect(hitEffectType, hit.point, Quaternion.LookRotation(hit.normal));
                    currentHitTarget = hit.transform;
                    lastHitTime = Time.time;
                }
            }
            else
            {
                currentHitTarget = null;
            }
        }

        public override void OnTriggerUp(WeaponSystem owner, Transform muzzle, Transform target)
        {
            // ビームのインスタンスを破棄
            if (currentBeamInstance != null)
            {
                Destroy(currentBeamInstance);
                currentBeamInstance = null;
                currentHitTarget = null;
            }
        }
    }
}

// using UnityEngine;
// using UnityEngine.VFX;
//
//
// namespace MyMaterials.Scripts.Weapon
// {
//     [CreateAssetMenu(menuName = "Weapons/Behavior/StylizedBeam")]
//     public class StylizedBeamBehavior : WeaponBehavior, IChargeWeapon
//     {
//         //todo 後々にScriptableObjectから参照にする
//         [Header("Settings")]
//         [SerializeField] private float damage = 10f;
//         [SerializeField] private float shootRange = 50f;
//         [SerializeField] private float shootInterval = 0.1f;
//         [SerializeField] private float maxCharge = 100f; 
//     
//         [Header("Effects")]
//         [SerializeField] private GameObject bulletLinePrefab;
//         [SerializeField] private float bulletLineLifetime = 1.0f;
//         [SerializeField] private Vector3 bulletLineOffset;
//         
//         [SerializeField] private GameObject muzzleFlashPrefab;
//         [SerializeField] private float muzzleFlashLifetime = 0.5f;
//         [SerializeField] private Vector3 muzzleFlashOffset;
//         
//         [SerializeField] private GameObject hitEffectPrefab;
//         [SerializeField] private Vector3 hitEffectOffset;
//
//         public float MaxCharge => maxCharge;
//         
//         private bool isFiring = false;
//         private float lastFireTime = 0f;
//         private GameObject hitEffect;
//         private GameObject muzzleFlash;
//         private GameObject bulletLine;
//         
//         private int layerMask01 = 1 << 11; //Layer10までを無視 
//         public override void OnTriggerDown(WeaponSystem owner, Transform muzzle, Transform target)
//         {
//             //引き金を引いた瞬間に発射
//             isFiring = true;
//             Shoot(muzzle);
//             lastFireTime = Time.time;
//         }
//
//         public override void OnTriggerHold(WeaponSystem owner, Transform muzzle, Transform target)
//         {
//             if(!isFiring) return;
//             if (Time.time - lastFireTime >= shootInterval)
//             {
//                 Shoot(muzzle);
//                 lastFireTime = Time.time;
//             }
//         }
//
//         public override void OnTriggerUp(WeaponSystem owner, Transform muzzle, Transform target)
//         {
//             isFiring = false;
//         }
//
//         private void Shoot(Transform muzzle)
//         {
//             Ray ray = new Ray(muzzle.position, muzzle.forward);
//             //Layer10までを無視
//             if (Physics.Raycast(ray, out RaycastHit hit, shootRange, layerMask01))
//             {
//                 //ヒットエフェクトの生成
//                 if (hitEffectPrefab)
//                 {
//                     if (hitEffect)
//                     {
//                         hitEffect.transform.position = hit.point + hitEffectOffset;
//                         hitEffect.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
//                         hitEffect.SetActive(true);
//                     }
//                     else
//                     {
//                         hitEffect = Instantiate(hitEffectPrefab, hit.point + hitEffectOffset, Quaternion.identity);
//                     }
//                 }
//             
//                 //ダメージ処理
//                 var damageble = hit.collider.GetComponent<IDamageable>();
//                 if (damageble != null)
//                 {
//                     Vector3 hitPoint = hit.point;
//                     Vector3 hitDirection = ray.direction;
//
//                     damageble.TakeDamage(damage, hitPoint, hitDirection);
//                 }
//             }
//             
//             SpawnMuzzleFlash(muzzle);
//             SpawnBulletLineVFX(muzzle);
//         }
//         
//         private void SpawnMuzzleFlash(Transform muzzle)
//         {
//             if(muzzleFlashPrefab == null) return;
//             var go = Instantiate(
//                 muzzleFlashPrefab,
//                 muzzle.position + muzzleFlashOffset,
//                 muzzle.rotation,
//                 muzzle
//             );
//             Destroy(go,muzzleFlashLifetime);
//         }
//         
//         private void SpawnBulletLineVFX(Transform muzzle)
//         {
//             if (bulletLinePrefab == null) return;
//             // 毎回インスタンス化して確実に最初から再生
//             var go = Instantiate(
//                 bulletLinePrefab,
//                 muzzle.position + bulletLineOffset,
//                 muzzle.rotation,
//                 muzzle
//             );
//             var vfx = go.GetComponent<VisualEffect>();
//             if (vfx != null)
//             {
//                 vfx.Stop();  // 念のため既存再生をリセット
//                 vfx.Play();  // 再生開始
//             }
//             // bulletLineLifetime 秒後にオブジェクトごと破棄
//             Destroy(go, bulletLineLifetime);
//         }
//     }
// }