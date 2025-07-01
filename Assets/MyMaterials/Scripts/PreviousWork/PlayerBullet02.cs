// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Random = UnityEngine.Random;
//
//
// public class PlayerBullet02 : MonoBehaviour
// {
//     [SerializeField] private float bulletSpeed;
//     [SerializeField] private float bulletDamage;
//     [SerializeField] private float exploreRadius;
//     [SerializeField] private GameObject hitEffectPrefab;
//     [SerializeField] private Vector3 hitEffectOffset;
//
//     private int layerMask01 = 1 << 11; //Layer10までを無視
//
//     private Rigidbody myrigid;
//     private GameObject target = null;
//     private bool isTarget = false;
//     private Vector3 randomDirection;
//     
//     void Start()
//     {
//         myrigid = this.gameObject.GetComponent<Rigidbody>();
//         randomDirection = GetRandomDirection();
//     }
//
//
//     void Update()
//     {
//         if (!isTarget)
//         {
//             target = GetMostCloseEnemy(exploreRadius);
//         }
//         
//         Move();
//     }a
//
//     GameObject GetMostCloseEnemy(float exploreRadius)
//     {
//         
//         //Layer11までを無視
//         Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.transform.position, exploreRadius, layerMask01);
//         
//         //Debug.Log(hitColliders.Length);
//         
//         //オブジェクトがない場合nullを返す
//         if (hitColliders.Length == 0)
//         {
//             return null;
//         }
//
//         float minDistance = 0.0f;
//
//         GameObject mostCloseEnemy = null;
//
//         minDistance = Vector3.Distance(hitColliders[0].gameObject.transform.position,
//             this.gameObject.transform.position);
//
//         foreach (var hitCollier in hitColliders)
//         {
//             float distance = Vector3.Distance(hitCollier.gameObject.transform.position,
//                 this.gameObject.transform.position);
//
//             if (minDistance > distance)
//             {
//                 minDistance = distance;
//                 mostCloseEnemy = hitCollier.gameObject;
//             }
//         }
//
//         isTarget = true;
//         return mostCloseEnemy;
//     }
//     
//     //最も近い敵に向かって加速 → ターゲットを固定したほうが良い
//     void Move()
//     {
//         if (target != null)
//         {
//             //targetEnemyを向くベクトルを生成
//             Vector3 targetVector = target.transform.position - this.gameObject.transform.position;
//             
//             myrigid.AddForce(targetVector.normalized * bulletSpeed);
//         }
//         else
//         {
//             myrigid.AddForce(randomDirection * 30);
//         }
//     }
//
//     void OnTriggerEnter(Collider other)
//     {
//         if (other.gameObject.CompareTag("Enemy"))
//         {
//             other.gameObject.GetComponent<Enemy>().RecieveDamage(bulletDamage);
//
//             StartCoroutine(KillMe(other.gameObject));
//         }
//     }
//
//     IEnumerator KillMe(GameObject enemy)
//     {
//         GameObject hitEffect = Instantiate(hitEffectPrefab, enemy.transform.position+hitEffectOffset, Quaternion.identity);
//
//         yield return new WaitForSeconds(0.1f);
//         
//         Destroy(hitEffect);
//         
//         //またなくてもいいかも
//         //yield return new WaitForSeconds(0.1f);
//         
//         Destroy(this.gameObject);
//     }
//
//
//     Vector3 GetRandomDirection()
//     {
//         float x = Random.Range(-1f, 1f);
//         float y = Random.Range(-1f, 1f);
//         float z = Random.Range(0f, 1f);
//
//         Vector3 randomVector = new Vector3(x, y, z);
//
//         return randomVector.normalized;
//     }
// }   