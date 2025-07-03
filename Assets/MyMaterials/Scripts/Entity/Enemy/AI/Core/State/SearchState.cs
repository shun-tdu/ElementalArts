using MyMaterials.Scripts.Entity.Enemy.AI.Movement;
using UnityEngine;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Core.State
{
    [CreateAssetMenu(fileName = "SearchState", menuName = "AI/States/Search")]
    public class SearchState : ScriptableObject, IEnemyState
    {
        [SerializeField] private float searchRotationSpeed = 30f;
        
        private EnemyMovement movement;

        public void OnEnter(EnemyAIController enemy)
        {
            Debug.Log("索敵状態に移行");
            
            // Stateに必要なコンポーネントを取得
            if (movement == null)
            {
                movement = enemy.GetComponent<EnemyMovement>();
            }
            
            //Stateに必要なコンポーネントがアタッチされていなければエラーを出力
            if (movement == null)
            {
                Debug.LogError("Search requires an EnemyMovement component on the enemy, but it's missing!", enemy.gameObject);
            }
        }

        public void OnUpdate(EnemyAIController enemy)
        {
            //プレイヤーが視野範囲にいなければなにもしない
            if (enemy.Vision.PlayerTarget  == null) return;
            
            //視野範囲にいた場合、プレイヤーの方向を向く
            movement?.LockAt(enemy.Vision.PlayerTarget.position);
        }
        
        public void OnExit(EnemyAIController enemy) { }
    }
}