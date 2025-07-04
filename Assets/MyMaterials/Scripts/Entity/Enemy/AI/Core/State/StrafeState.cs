using MyMaterials.Scripts.Entity.Enemy.AI.Movement;
using MyMaterials.Scripts.Entity.Enemy.AI.Weapons;
using UnityEngine;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Core.State
{
    [CreateAssetMenu(fileName = "StrafeState", menuName = "AI/States/Strafe")]
    public class StrafeState : ScriptableObject, IEnemyState
    {
        private EnemyStrafeMovement movement;
        public EnemyWeapons weapons;

        public void OnEnter(EnemyAIController enemy)
        {
            Debug.Log("ストレイフ状態にに移行");
            // Stateに必要なコンポーネントを取得
            if (movement == null)
            {
                movement = enemy.GetComponent<EnemyStrafeMovement>();
            }
            if (weapons == null)
            {
                weapons = enemy.GetComponent<EnemyWeapons>();
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
            if(enemy.Vision.PlayerTarget == null) return;
            
            movement?.Strafe(enemy.Vision.PlayerTarget);
            
            weapons?.TryFire();
        }

        public void OnExit(EnemyAIController enemy)
        {
            movement?.Stop();
        }
    }
}