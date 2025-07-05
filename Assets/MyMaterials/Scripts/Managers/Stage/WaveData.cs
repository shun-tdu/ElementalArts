using System.Collections.Generic;
using UnityEngine;

namespace MyMaterials.Scripts.Managers.Stage
{
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public GameObject enemyPrefab;  // 出現させる敵のプレハブ
        public int count = 1;           // この敵を何体出すか
        public float spawnDelay = 1.0f; // この敵を出す前の待機時間
    }
    
    [CreateAssetMenu(fileName = "WaveData_", menuName = "Stage/Wave Data")]
    public class WaveData : ScriptableObject
    {
        [Header("ウェーブ設定")] 
        public List<EnemySpawnInfo> enemiesToSpawn;       // このウェーブで出現する敵のリスト

        [Header("次のウェーブへの移行条件")] 
        public bool waitForAllEnemiesDefeated = true;   // このウェーブの敵を全滅させるまで待つか
        public float delayToNextWave = 5.0f;            // 次のウェーブまでの待機時間
    }
}