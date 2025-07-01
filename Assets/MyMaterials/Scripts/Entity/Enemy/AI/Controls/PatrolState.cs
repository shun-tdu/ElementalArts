// using UnityEngine;
//
// namespace MyMaterials.Scripts.Entity.Enemy.AI.Controls
// {
//     public class PatrolState : IEnemyState
//     {
//         public void OnEnter(StationaryEnemyAIController stationaryEnemy)
//         {
//             Debug.Log("索敵状態に移行");
//             // 例: 移動速度を巡回モードに設定
//             // enemy.Movement.SetSpeed(enemy.patrolSpeed);
//         }
//
//         public void OnUpdate(StationaryEnemyAIController stationaryEnemy)
//         {
//             // プレイヤーが追跡範囲内に入ったら、追跡状態に切り替える
//             if (stationaryEnemy.IsInChaseRange())
//             {
//                 stationaryEnemy.ChangeState(stationaryEnemy.ChaseState);
//                 return;
//             }
//             
//             
//             // 索敵ルートを巡回する処理
//             stationaryEnemy.Movement.Patrol();
//         }
//         
//         public void OnExit(StationaryEnemyAIController stationaryEnemy)
//         {
//             Debug.Log("索敵状態を終了");
//         }
//     }
// }