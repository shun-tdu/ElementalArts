using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;
using Cysharp.Threading.Tasks;

namespace MyMaterials.Scripts.Singletons
{
    // エフェクトの種類
    public enum EffectType
    {
        // Muzzle Flash
        Laser,

        // HitEffect
        Explode,
    }

    public class EffectManager : MonoBehaviour
    {
        public static EffectManager Instance { get; private set; }

        [Header("Effect Pool Settings")]
        [SerializeField] private int effectPoolSize = 64;

        [Header("Effect Information")] 
        [SerializeField] private List<EffectInformation> effectInformations;
        [System.Serializable]
        public class EffectInformation
        {
            public EffectType type;
            public GameObject prefab;
            public float duration = 5f;
        }
        
        private Dictionary<EffectType, Queue<GameObject>> effectPool;
         
        private void Awake()
        {
            //シングルトン化
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            // EffectPoolの初期化
            InitializePool();
        }
        
        
        /// <summary>
        /// オブジェクトプールを初期化する
        /// </summary>
        private void InitializePool()
        {
            effectPool = new Dictionary<EffectType, Queue<GameObject>>();

            foreach (var effectInfo in effectInformations)
            {
                var objectQueue = new Queue<GameObject>();

                for (int i = 0; i < effectPoolSize; i++)
                {
                    GameObject effectInstance = Instantiate(effectInfo.prefab, transform);
                    
                    effectInstance.SetActive(false);
                    
                    objectQueue.Enqueue(effectInstance);
                }
                effectPool[effectInfo.type] = objectQueue;
            }
        }
        
        /// <summary>
        /// 指定した座標、回転でVFXグラフを再生する
        /// </summary>
        /// <param name="type">エフェクトタイプ</param>
        /// <param name="position">エフェクトを生成をする座標</param>
        /// <param name="rotation">エフェクトの回転</param>
        public void PlayEffect(EffectType type, Vector3 position, Quaternion rotation)
        {
            if (!effectPool.ContainsKey(type))
            {
                Debug.LogWarning("EffectType: {type} not found in pool");
                return;
            }

            Queue<GameObject> objectQueue = effectPool[type];
            GameObject effectToPlay;
            
            // プールをに待機中のオブジェクトを取り出す
            if (objectQueue.Count > 0)
            {
                effectToPlay = objectQueue.Dequeue();
            }
            // 待機中のオブジェクトがない場合は、新しく生成
            else
            {
                var effectInfo = effectInformations.Find(info => info.type == type);
                effectToPlay = Instantiate(effectInfo.prefab, transform);
                Debug.LogWarning($"Effect pool for {type} was empty. Instantiated a new one.");
            }
            
            // エフェクトの位置と向きを設定
            effectToPlay.transform.position = position;
            effectToPlay.transform.rotation = rotation;
            
            effectToPlay.SetActive(true);
            var vfx = effectToPlay.GetComponent<VisualEffect>();
            if (vfx != null)
            {
                vfx.Play();
            }
            
            // 再生終了後にプールに戻す非同期処理を開始
            var info = effectInformations.Find(i => i.type == type);
            ReturnToPoolAfterDelay(type, effectToPlay, info.duration).Forget();
        }

        /// <summary>
        /// 指定時間後にエフェクトを非アクティブ化し、プールに戻す
        /// </summary>
        private async UniTaskVoid ReturnToPoolAfterDelay(EffectType type, GameObject effectInstance, float duration)
        {
            // このオブジェクトが破棄されたらタスクも自動でキャンセルされる
            var token = effectInstance.GetCancellationTokenOnDestroy();

            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);

            // UniTaskがキャンセルされていなければ（オブジェクトがまだ生きていれば）プールに戻す
            if (!token.IsCancellationRequested)
            {
                effectInstance.SetActive(false);
                effectPool[type].Enqueue(effectInstance);
            }
        }
    }
}