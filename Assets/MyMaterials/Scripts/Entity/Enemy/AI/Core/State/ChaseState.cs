using MyMaterials.Scripts.Entity.Enemy.AI.Movement;
using UnityEngine;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Core.State
{
    [CreateAssetMenu(fileName = "ChaseState", menuName = "AI/States/Chase")]
    public class ChaseState : ScriptableObject, IEnemyState
    {
        private EnemyMovement movement;
        
        public void OnEnter(EnemyAIController enemy)
        {
            // Debug.Log("追跡状態に移行");
            
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
            //プレイヤーが視野範囲にいなければなにもしない
            if(enemy.Vision.PlayerTarget == null) return;
            
            // プレイヤーを追いかける処理
            movement?.MoveTowards(enemy.Vision.PlayerTarget.position);
        }

        public void OnExit(EnemyAIController enemy)
        {
            // Debug.Log("追跡状態を終了");
        }
    }
}