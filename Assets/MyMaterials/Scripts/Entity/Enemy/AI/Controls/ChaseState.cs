// using UnityEngine;
//
// namespace MyMaterials.Scripts.Entity.Enemy.AI.Controls
// {
//     public class ChaseState : IEnemyState
//     {
//         public void OnEnter(StationaryEnemyAIController stationaryEnemy)
//         {
//             Debug.Log("追跡状態に移行");
//             // 例: 移動速度を追跡モードに設定
//             // enemy.Movement.SetSpeed(enemy.chaseSpeed);
//         }
//
//         public void OnUpdate(StationaryEnemyAIController stationaryEnemy)
//         {
//             // プレイヤーが攻撃範囲内に入ったら、攻撃状態に切り替える
//             if (stationaryEnemy.IsInAttackRange())
//             {
//                 stationaryEnemy.ChangeState(stationaryEnemy.AttackState);
//                 return;
//             }
//         
//             // プレイヤーが追跡範囲から出たら、索敵状態に戻る
//             if (!stationaryEnemy.IsInChaseRange())
//             {
//                 stationaryEnemy.ChangeState(stationaryEnemy.PatrolState);
//                 return;
//             }
//
//             // プレイヤーを追いかける処理
//             stationaryEnemy.Movement.MoveTowards(stationaryEnemy.PlayerTarget.position);
//         }
//
//         public void OnExit(StationaryEnemyAIController stationaryEnemy)
//         {
//             Debug.Log("追跡状態を終了");
//         }
//     }
// }