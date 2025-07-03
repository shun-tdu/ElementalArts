using UnityEngine;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Core.Conditions
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "IsCloseToPlayerCondition", menuName = "AI/Conditions/Is Close To Player")]
    public class IsCloseToPlayerCondition : ScriptableObject, IStateCondition
    {
        public bool IsMet(EnemyAIController enemy, AIPersonality personality)
        {
            //視野範囲内にプレイヤーがいるか
            if (enemy.Vision.PlayerTarget == null) return false;
            
            //プレイヤーとの距離が攻撃範囲内か
            float distance = Vector3.Distance(enemy.transform.position, enemy.Vision.PlayerTarget.position);
            return distance < personality.attackRange;
        }
    }
}