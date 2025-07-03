using UnityEngine;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Core.Conditions
{
    [CreateAssetMenu(fileName = "IsPlayerInLongRangeCondition", menuName = "AI/Conditions/Is Player In Long Range")]
    public class IsPlayerInLongRangeCondition : ScriptableObject, IStateCondition
    {
        public bool IsMet(EnemyAIController enemy, AIPersonality personality)
        {
            if (enemy.Vision.PlayerTarget == null) return false;
            
            // プレイヤーとの距離を計算
            float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.Vision.PlayerTarget.position);
            
            // プレイヤーが遠距離攻撃範囲内かつ近距離攻撃範囲外にいるかチェック
            bool isInLongRange = distanceToPlayer <= personality.longRangeAttackRange;
            bool isOutSideShortRange = distanceToPlayer > personality.attackRange;

            // 両方の条件を満たしていればtrueを返す
            return isInLongRange && isOutSideShortRange;
        }
    }
}