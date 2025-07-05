using UnityEngine;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Core.Conditions
{
    /// <summary>
    /// デフォルトの状態遷移条件
    /// 必ずTrueになることで、特定のStateに入ることを保証する
    /// 通常、Personalityの遷移リストの最後に設定する
    /// </summary>
    [CreateAssetMenu(fileName = "DefaultCondition", menuName = "AI/Conditions/Default")]
    public class DefaultCondition : ScriptableObject, IStateCondition
    {
        public bool IsMet(EnemyAIController enemy, AIPersonality personality)
        {
            return true;
        }
    }
}