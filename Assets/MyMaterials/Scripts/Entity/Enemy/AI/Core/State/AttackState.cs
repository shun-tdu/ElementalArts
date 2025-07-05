using MyMaterials.Scripts.Entity.Enemy.AI.Movement;
using MyMaterials.Scripts.Entity.Enemy.AI.Weapons;
using MyMaterials.Scripts.Managers.Singletons;
using UnityEngine;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Core.State
{
    [CreateAssetMenu(fileName = "AttackState", menuName = "AI/States/Attack")]
    public class AttackState:ScriptableObject, IEnemyState
    {
        private EnemyWeapons weapons;
        private EnemyMovement movement;
        
        public void OnEnter(EnemyAIController enemy)
        {
            // Debug.Log("攻撃状態に移行");
            
            // Stateに必要なコンポーネントを取得
            if (weapons == null)
            {
                weapons = enemy.GetComponent<EnemyWeapons>();
            }

            if (movement == null)
            {
                movement = enemy.GetComponent<EnemyMovement>();
            }
            
            // Stateに必要なコンポーネントがアタッチされていなければエラーを出力
            if (weapons == null)
            {
                Debug.LogError("AttackState requires an EnemyWeapon component on the enemy, but it's missing!", enemy.gameObject);
            }
            if (movement == null)
            {
                Debug.LogError("PatrolState requires an EnemyMovement component on the enemy, but it's missing!", enemy.gameObject);
            }
        }

        public void OnUpdate(EnemyAIController enemy)
        {
            if (enemy.Vision.PlayerTarget == null) return;
            
            // ターゲットの方向を向き続ける
            movement?.LockAt(enemy.Vision.PlayerTarget.position);
            
            // 一定間隔でバースト射撃
            weapons?.TryFire();
        }

        public void OnExit(EnemyAIController enemy) { }
    }
}