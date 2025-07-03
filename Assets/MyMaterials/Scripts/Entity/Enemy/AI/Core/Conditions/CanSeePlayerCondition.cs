using UnityEngine;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Core.Conditions
{
    [CreateAssetMenu(fileName = "CanSeePlayerCondition", menuName = "AI/Conditions/Can See Player")]
    public class CanSeePlayerCondition : ScriptableObject, IStateCondition
    {
        public bool IsMet(EnemyAIController enemy, AIPersonality personality)
        {
            //視野範囲にプレイヤーがいるかを確認
            Transform player = enemy.Vision.FindPlayerInView();
            
            //視野範囲内にいた場合はターゲットを更新
            enemy.Vision.PlayerTarget = player;

            return player != null;
        }
    }
}