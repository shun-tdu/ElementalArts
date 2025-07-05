using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Cysharp.Threading.Tasks;

using MyMaterials.Scripts.Entity.Enemy;
using MyMaterials.Scripts.Managers.Stage;
using MyMaterials.Scripts.Managers.Singletons;

namespace MyMaterials.Scripts.Managers
{
    public class StageManager : MonoBehaviour
    {
        [Header("ステージ設定")] [SerializeField] private List<WaveData> waves;
        [SerializeField] private List<EnemySpawner> spawners;
        [SerializeField] private float stageStartDelay = 1f;

        private List<EnemyHealth> activeEnemies = new List<EnemyHealth>();

        [Header("BGM設定")] [SerializeField] private AudioClip stageBgm;
        [SerializeField] private float bgmFadeInTime = 2.0f;

        private async void Start()
        {
            if (stageBgm != null)
            {
                AudioManager.Instance.PlayBGM(stageBgm, bgmFadeInTime);
            }
            
            await UniTask.Delay(TimeSpan.FromSeconds(stageStartDelay),
                cancellationToken: this.GetCancellationTokenOnDestroy());
            await ProcessWaves();
        }

        /// <summary>
        /// ウェーブを順番に処理していく非同期メソッド
        /// </summary>
        private async UniTask ProcessWaves()
        {
            int waveIndex = 0;
            foreach (var wave in waves)
            {
                Debug.Log($"ウェーブ {waveIndex + 1} スタート");
                
                // これにより、全ての敵が出現し終わるまで、次の行には進まない
                await SpawnWave(wave);

                // このウェーブの敵が全滅するまで待つ
                if (wave.waitForAllEnemiesDefeated)
                {
                    await UniTask.WaitUntil(() => activeEnemies.Count == 0,
                        cancellationToken: this.GetCancellationTokenOnDestroy());
                    Debug.Log($"ウェーブ {waveIndex + 1} 完了");
                }

                // 次のウェーブまでの待機時間
                await UniTask.Delay(TimeSpan.FromSeconds(wave.delayToNextWave),
                    cancellationToken: this.GetCancellationTokenOnDestroy());
                waveIndex++;
            }

            Debug.Log("ステージクリア！");
            //GameManager.Instance.ChangeState(GameState.StageClear);
        }


        /// <summary>
        /// 1つのウェーブで定義された敵を順番に出現させる
        /// </summary>
        // ★★★ 戻り値をUniTaskVoidからUniTaskに変更 ★★★
        private async UniTask SpawnWave(WaveData waveData)
        {
            if (waveData == null) return;

            foreach (var spawnInfo in waveData.enemiesToSpawn)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(spawnInfo.spawnDelay),
                    cancellationToken: this.GetCancellationTokenOnDestroy());

                for (int i = 0; i < spawnInfo.count; i++)
                {
                    if (spawners == null || spawners.Count == 0) continue;

                    EnemySpawner spawner = spawners[Random.Range(0, spawners.Count)];
                    GameObject enemyInstance = spawner.Spawn(spawnInfo.enemyPrefab);

                    if (enemyInstance != null && enemyInstance.TryGetComponent<EnemyHealth>(out var enemyHealth))
                    {
                        activeEnemies.Add(enemyHealth);

                        enemyHealth.OnDeath += OnEnemyDefeated;
                    }
                }
            }
        }


        /// <summary>
        /// 敵が倒されたときに呼ばれるコールバック
        /// </summary>
        private void OnEnemyDefeated(EnemyHealth defeatedEnemy)
        {
            if (defeatedEnemy != null)
            {
                defeatedEnemy.OnDeath -= OnEnemyDefeated;
                activeEnemies.Remove(defeatedEnemy);
            }
        }
    }
}