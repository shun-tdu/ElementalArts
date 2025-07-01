using UnityEngine;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Controls
{
    public class IdleState : IEnemyState
    {
        public void OnEnter(StationaryEnemyAIController enemy)
        {
            Debug.Log("待機状態に移行");
        }

        public void OnUpdate(StationaryEnemyAIController enemy)
        {
            enemy.ChangeState(enemy.SearchState);
        }
        
        public void OnExit(StationaryEnemyAIController enemy) {}
    }
}