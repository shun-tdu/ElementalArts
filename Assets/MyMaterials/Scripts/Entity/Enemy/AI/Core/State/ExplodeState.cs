using UnityEngine;
using MyMaterials.Scripts.Entity.Enemy.AI.Weapons;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Core.State
{
    [CreateAssetMenu(fileName = "ExplodeState", menuName = "AI/States/Explode")]
    public class ExplodeState : ScriptableObject, IEnemyState
    {
        public void OnEnter(EnemyAIController enemy)
        {
            var explosion = enemy.GetComponent<EnemyExplosion>();

            if (explosion != null)
            {
                explosion.Explode();
            }
            else
            {
                Debug.LogError("ExplodeState requires an ExplosionDamage component on the enemy, but it's missing!", enemy.gameObject);
            }
        }
        public void OnUpdate(EnemyAIController enemy) { }
        public void OnExit(EnemyAIController enemy) { }
    }
}