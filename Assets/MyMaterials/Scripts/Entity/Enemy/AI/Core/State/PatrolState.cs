using MyMaterials.Scripts.Entity.Enemy.AI.Movement;
using UnityEngine;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Core.State
{
    [CreateAssetMenu(fileName = "PatrolState", menuName = "AI/States/Patrol")]
    public class PatrolState : ScriptableObject, IEnemyState
    {
        private EnemyMovement movement;
        
        public void OnEnter(EnemyAIController enemy)
        {
            Debug.Log("索敵状態に移行");

            //Stateに必要なコンポーネントを取得
            if (movement == null)
            {
                movement = enemy.GetComponent<EnemyMovement>();
            }
            
            //Stateに必要なコンポーネントがアタッチされていなければエラーを出力
            if (movement == null)
            {
                Debug.LogError("PatrolState requires an EnemyMovement component on the enemy, but it's missing!", enemy.gameObject);
            }
        }

        public void OnUpdate(EnemyAIController enemy)
        {
            movement?.Patrol();
        }
        
        public void OnExit(EnemyAIController enemy)
        {
            Debug.Log("索敵状態を終了");
        }
    }
}