namespace MyMaterials.Scripts.Entity.Enemy.AI.Core.Conditions
{
    /// <summary>
    /// 全EnemyのConditionが持つべきインターフェース
    /// </summary>
    public interface IStateCondition
    {
        bool IsMet(EnemyAIController enemy, AIPersonality personality);
    }
}