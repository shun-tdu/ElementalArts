namespace MyMaterials.Scripts.Entity.Enemy.AI.Core.State
{
    /// <summary>
    /// すべての敵AIをが実装すべきインターフェース
    /// </summary>
    public interface IEnemyState
    {
        /// <summary>
        /// この状態に遷移したときに一度だけ呼ばれる処理
        /// </summary>
        /// <param name="enemy">AIのコントローラー</param>
        void OnEnter(EnemyAIController enemy);

        /// <summary>
        /// この状態である間、毎フレーム呼ばれる処理
        /// </summary>
        /// <param name="enemy"></param>
        void OnUpdate(EnemyAIController enemy);

        /// <summary>
        /// この状態から別の状態に遷移するときに一度だけ呼ばれる処理
        /// </summary>
        /// <param name="enemy"></param>
        void OnExit(EnemyAIController enemy);
    }
}