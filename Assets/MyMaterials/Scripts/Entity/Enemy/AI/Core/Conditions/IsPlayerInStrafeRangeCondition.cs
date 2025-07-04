using UnityEngine;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Core.Conditions
{
    [CreateAssetMenu(fileName = "IsPlayerInStrafeRangeCondition", menuName = "AI/Conditions/Is Player In Strafe Range")]
    public class IsPlayerInStrafeRangeCondition : ScriptableObject, IStateCondition
    {
        public bool IsMet(EnemyAIController enemy, AIPersonality personality)
        {
            // プレイヤーを失っていたら条件を満たさない
            Transform player = enemy.Vision.FindPlayerInView();
            if (player == null) return false;

            // ターゲット情報を更新
            enemy.Vision.PlayerTarget = player;

            // プレイヤーとの現在の距離を計算
            float distanceToPlayer = Vector3.Distance(enemy.transform.position, player.position);

            // プレイヤーが遠距離攻撃範囲内かつ近距離攻撃範囲外にいるかチェック
            bool isInLongRange = distanceToPlayer <= personality.longRangeAttackRange;
            bool isOutsideShotRange = distanceToPlayer > personality.attackRange;

            // 両方の条件を満たしていればtrueを返す
            return isInLongRange && isOutsideShotRange;
        }
    }
}