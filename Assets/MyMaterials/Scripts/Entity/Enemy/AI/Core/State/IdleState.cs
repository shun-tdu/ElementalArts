// using UnityEngine;
//
// namespace MyMaterials.Scripts.Entity.Enemy.AI.Core.State
// {
//     public class IdleState : IEnemyState
//     {
//         public void OnEnter(EnemyAIController enemy)
//         {
//             Debug.Log("待機状態に移行");
//         }
//
//         public void OnUpdate(EnemyAIController enemy)
//         {
//             enemy.ChangeState(enemy.SearchState);
//         }
//         
//         public void OnExit(EnemyAIController enemy) {}
//     }
// }