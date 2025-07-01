namespace MyMaterials.Scripts.Entity.Enemy.AI.Controls
{
    /// <summary>
    /// すべての敵AIをが実装すべきインターフェース
    /// </summary>
    public interface IEnemyState
    {
        /// <summary>
        /// この状態に遷移したときに一度だけ呼ばれる処理
        /// </summary>
        /// <param name="stationaryEnemy">AIのコントローラー</param>
        void OnEnter(StationaryEnemyAIController stationaryEnemy);

        /// <summary>
        /// この状態である間、毎フレーム呼ばれる処理
        /// </summary>
        /// <param name="stationaryEnemy"></param>
        void OnUpdate(StationaryEnemyAIController stationaryEnemy);

        /// <summary>
        /// この状態から別の状態に遷移するときに一度だけ呼ばれる処理
        /// </summary>
        /// <param name="stationaryEnemy"></param>
        void OnExit(StationaryEnemyAIController stationaryEnemy);
    }
}