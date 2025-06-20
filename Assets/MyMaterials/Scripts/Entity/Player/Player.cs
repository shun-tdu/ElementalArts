// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Unity.VisualScripting;
// using UnityEngine;
//
// namespace Player
// {
//     public class Player : MonoBehaviour
//     {
//         [SerializeField] private float playerHp = 100.0f;
//         [SerializeField] private int damage = 10;
//         [SerializeField] private int maxAmmo = 100;
//         [SerializeField] private float rotationSpeed = 0.1f;
//         [SerializeField] private float targetAngle = 45;
//         [SerializeField] private float shootInterval = 0.1f;
//         [SerializeField] private float shootRange = 50;
//         [SerializeField] private float absorbableRadius = 0.0f;
//
//         [SerializeField] private Vector3 limitVel;
//         [SerializeField] private Vector3 muzzleFlashScale;
//         [SerializeField] private Vector3 bulletLineScale;
//         [SerializeField] private Vector3 muzzleFlashOffset;
//         [SerializeField] private Vector3 bulletLineOffset;
//         [SerializeField] private Vector3 hitEffectOffset;
//
//         [SerializeField] private GameObject muzzleFlashPrefab;
//         [SerializeField] private GameObject bulletLinePrefab;
//         [SerializeField] private GameObject hitEffectPrefab;
//         [SerializeField] private GameObject secondaryWeaponPrefab;
//
//
//         private float absorbedEnergy = 0.0f;
//         private float limitPosX = 74;
//
//         private int layerMask01 = 1 << 11; //Layer10までを無視
//         private int layerMask02 = 1 << 12; //Layer11までを無視
//
//         private bool is_shoot = false;
//         private int ammo;
//         private GameObject muzzleFlash;
//         private GameObject bulletLine;
//         private GameObject hitEffect;
//         private Rigidbody myRigid;
//         private Transform myTransform;
//         private GameObject eventManagerObj;
//         private EventManager eventManager;
//
//
//         void Start()
//         {
//             InitGun();
//             myRigid = this.gameObject.GetComponent<Rigidbody>();
//             myTransform = this.gameObject.GetComponent<Transform>();
//             if (GameObject.Find("EventManager"))
//             {
//                 eventManagerObj = GameObject.Find("EventManager");
//                 eventManager = eventManagerObj.GetComponent<EventManager>();
//             }
//         }
//
//         void Update()
//         {
//             CalcAbsorbedEnergy();
//             MovePlayer();
//
//             if (Input.GetKey(KeyCode.Mouse0))
//             {
//                 StartCoroutine(ShootTimer());
//             }
//
//             if (Input.GetKeyDown(KeyCode.C))
//             {
//                 ShotSecondaryWeapon();
//             }
//         }
//
//
//         //移動関数
//         private void MovePlayer()
//         {
//             if (Input.GetKey(KeyCode.W))
//             {
//                 myRigid.AddForce(Vector3.forward);
//             }
//
//             if (Input.GetKey(KeyCode.A))
//             {
//                 myRigid.AddForce(Vector3.left);
//                 Quaternion to_qua = Quaternion.AngleAxis(targetAngle, Vector3.forward);
//                 myTransform.rotation = Quaternion.Slerp(myTransform.rotation, to_qua, Time.deltaTime * rotationSpeed);
//             }
//             else
//             {
//                 Quaternion to_qua = Quaternion.AngleAxis(0, Vector3.forward);
//                 myTransform.rotation = Quaternion.Slerp(myTransform.rotation, to_qua, Time.deltaTime * rotationSpeed);
//             }
//
//             if (Input.GetKey(KeyCode.S))
//             {
//                 myRigid.AddForce(Vector3.back);
//             }
//
//             if (Input.GetKey(KeyCode.D))
//             {
//                 myRigid.AddForce(Vector3.right);
//                 Quaternion to_qua = Quaternion.AngleAxis(-targetAngle, Vector3.forward);
//                 myTransform.rotation = Quaternion.Slerp(myTransform.rotation, to_qua, Time.deltaTime * rotationSpeed);
//             }
//             else
//             {
//                 Quaternion to_qua = Quaternion.AngleAxis(0, Vector3.forward);
//                 myTransform.rotation = Quaternion.Slerp(myTransform.rotation, to_qua, Time.deltaTime * rotationSpeed);
//             }
//             //ブーストと回避を実装したい
//
//
//             //移動の制限
//             if (myTransform.position.x < -75)
//             {
//                 Vector3 pos = myTransform.position;
//                 pos.x = -limitPosX;
//                 myTransform.position = pos;
//
//                 Vector3 vel = myRigid.velocity;
//                 vel.x = 0;
//                 myRigid.velocity = vel;
//             }
//
//             if (myTransform.position.x > 75)
//             {
//                 Vector3 pos = myTransform.position;
//                 pos.x = limitPosX;
//                 myTransform.position = pos;
//
//                 Vector3 vel = myRigid.velocity;
//                 vel.x = 0;
//                 myRigid.velocity = vel;
//             }
//
//             //速度の制限
//             if (Mathf.Abs(myRigid.velocity.z) > limitVel.z)
//             {
//                 if (myRigid.velocity.z > 0)
//                 {
//                     Vector3 vel = myRigid.velocity;
//                     vel.z = limitVel.z;
//                     myRigid.velocity = vel;
//                 }
//                 else
//                 {
//                     Vector3 vel = myRigid.velocity;
//                     vel.z = -limitVel.z;
//                     myRigid.velocity = vel;
//                 }
//             }
//         }
//
//
//         //射撃関数
//         void Shoot()
//         {
//             Ray ray = new Ray(transform.position, transform.forward);
//             RaycastHit hit;
//             //Layer10までを無視
//             if (Physics.Raycast(ray, out hit, shootRange, layerMask01))
//             {
//                 //ヒットエフェクトの生成
//                 if (hitEffectPrefab != null)
//                 {
//                     if (hitEffect != null)
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
//                 if (hit.collider.CompareTag("Enemy"))
//                 {
//                     hit.collider.gameObject.GetComponent<Enemy>().RecieveDamage(damage * absorbedEnergy);
//                 }
//             }
//
//             Ammo--;
//         }
//
//         //Gunの初期化
//         void InitGun()
//         {
//             Ammo = maxAmmo;
//         }
//
//         //射撃コルーチン
//         IEnumerator ShootTimer()
//         {
//             if (!is_shoot)
//             {
//                 is_shoot = true;
//
//                 //マズルフラッシュの生成
//                 if (muzzleFlashPrefab != null)
//                 {
//                     if (muzzleFlash != null)
//                     {
//                         muzzleFlash.SetActive(true);
//                     }
//                     else
//                     {
//                         muzzleFlash = Instantiate(muzzleFlashPrefab, transform.position + muzzleFlashOffset,
//                             transform.rotation);
//                         muzzleFlash.transform.SetParent(gameObject.transform);
//                         muzzleFlash.transform.localScale = muzzleFlashScale;
//                     }
//                 }
//
//                 //弾道の生成
//                 if (bulletLinePrefab != null)
//                 {
//                     if (bulletLine != null)
//                     {
//                         bulletLine.SetActive(true);
//                     }
//                     else
//                     {
//                         bulletLine = Instantiate(bulletLinePrefab, transform.position + bulletLineOffset,
//                             transform.rotation);
//                         bulletLine.transform.SetParent(gameObject.transform);
//                         bulletLine.transform.localScale = bulletLineScale;
//                     }
//                 }
//
//                 //射撃処理
//                 Shoot();
//
//                 yield return new WaitForSeconds(shootInterval);
//
//                 //マズルフラッシュの非表示
//                 if (muzzleFlash != null)
//                 {
//                     muzzleFlash.SetActive(false);
//                 }
//
//                 //弾道の非表示
//                 if (bulletLine != null)
//                 {
//                     //ここに弾道の散逸エフェクトを出す
//                     bulletLine.SetActive(false);
//                 }
//
//                 //ヒットエフェクトの非表示
//                 if (hitEffect != null)
//                 {
//                     if (hitEffect.activeSelf)
//                     {
//                         hitEffect.SetActive(false);
//                     }
//                 }
//
//                 is_shoot = false;
//             }
//             else
//             {
//                 yield return null;
//             }
//         }
//
//         //特殊射撃
//         void ShotSecondaryWeapon()
//         {
//             //プレイヤーの向きを基準として3D扇状に射出
//
//             Vector3 ordinalVec = transform.up;
//
//             for (int i = 0; i < 4; i++)
//             {
//                 var result = Quaternion.Euler(0, 0, 90 * i) * ordinalVec;
//
//                 GameObject bullet = Instantiate(secondaryWeaponPrefab, transform.position, transform.rotation);
//
//                 Rigidbody rb = bullet.gameObject.GetComponent<Rigidbody>();
//
//                 rb.AddForce(-transform.forward * 30 + result * 10, ForceMode.Impulse);
//             }
//         }
//
//         //破壊処理
//         void Killed()
//         {
//         }
//
//         /*--------Calc EnergyLevel--------*/
//         //プレイヤーから一定距離内にある敵弾を下にEnegyを計算
//         float CalcBulletEnergy(float exploreRadius)
//         {
//             //Layer11までを無視
//             Collider[] hitColliders =
//                 Physics.OverlapSphere(this.gameObject.transform.position, exploreRadius, layerMask02);
//
//             float energy = 0f;
//
//             foreach (var hitCollier in hitColliders)
//             {
//                 float distance = Vector3.Distance(hitCollier.gameObject.transform.position,
//                     this.gameObject.transform.position);
//
//                 energy += 1 / distance;
//             }
//
//             return energy;
//         }
//
//         //AbsorbedEnergyの計算
//         void CalcAbsorbedEnergy()
//         {
//             //Energy計算の重み
//             float scoreWeight = 0.1f;
//             float enemyBullerWeight = 100;
//
//             absorbedEnergy = eventManager.GetGameScore() * scoreWeight +
//                              CalcBulletEnergy(absorbableRadius) * enemyBullerWeight;
//
//             eventManager.AddAbsorbedEnergy(absorbedEnergy);
//         }
//
//
//         /*--------Public Function--------*/
//         public float GetPlayerHp()
//         {
//             return playerHp;
//         }
//
//         public float GetAbsorbedEnergy()
//         {
//             return absorbedEnergy;
//         }
//
//         public int Ammo
//         {
//             set { ammo = Mathf.Clamp(value, 0, maxAmmo); }
//             get { return ammo; }
//         }
//
//         //被弾処理
//         public void RecieveDamage(float takenDamage)
//         {
//             playerHp -= takenDamage;
//             eventManager.ReducePlayerLife(takenDamage);
//
//             if (playerHp <= 0)
//             {
//                 Killed();
//             }
//         }
//     }
// }