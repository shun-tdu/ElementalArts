// using MyMaterials.Scripts.Singletons;
// using UnityEngine;
//
// namespace MyMaterials.Scripts.Entity.Enemy.AI.Core.State
// {
//     public class AttackState:IEnemyState
//     {
//         private float attackInterval = 1.5f;
//         private float attackTimer;
//         
//         public void OnEnter(EnemyAIController enemy)
//         {
//             Debug.Log("攻撃状態に移行");
//             attackTimer = 0f;
//         }
//
//         public void OnUpdate(EnemyAIController enemy)
//         {
//             if (enemy.PlayerTarget == null || !enemy.Vision.FindPlayerInView())
//             {
//                 enemy.ChangeState(enemy.SearchState);
//                 return;
//             }
//             
//             //ターゲットの方向を向き続ける
//             Vector3 direction = (enemy.PlayerTarget.position - enemy.transform.position).normalized;
//             Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
//             enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, lookRotation, Time.deltaTime * 5f);
//             
//             //一定間隔でバースト射撃
//             attackTimer += Time.deltaTime;
//             if (attackTimer >= attackInterval)
//             {
//                 enemy.Weapons.StartBurstFire();
//                 attackTimer = 0f;
//             }
//         }
//
//         public void OnExit(EnemyAIController enemy) { }
//     }
// }