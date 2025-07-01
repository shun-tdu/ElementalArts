using UnityEngine;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Controls
{
    public class SearchState : IEnemyState
    {
        private float searchRotationSpeed = 30f;

        public void OnEnter(StationaryEnemyAIController enemy)
        {
            Debug.Log("索敵状態に移行");
            enemy.PlayerTarget = null;
        }

        public void OnUpdate(StationaryEnemyAIController enemy)
        {
            Transform player = enemy.Vision.FindPlayerInView();
            if (player != null)
            {
                enemy.PlayerTarget = player;
                enemy.ChangeState(enemy.AttackState);
                return;
            }
            enemy.transform.Rotate(0,searchRotationSpeed * Time.deltaTime,0);
        }
        
        public void OnExit(StationaryEnemyAIController enemy) { }
    }
}