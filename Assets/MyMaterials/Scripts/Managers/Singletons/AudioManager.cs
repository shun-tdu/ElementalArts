using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace MyMaterials.Scripts.Managers.Singletons
{
    public enum SoundType
    {
        // Player Actions
        Step,
        DashStart,
        WeaponFire,
        EngineHum,
        //Lock-On System
        TargetAcquired,
        IntentionLockOn,
        TargetLost,
        //Weapon
        BeamGun_1,
        BeamGun_2,
        Explosion_1,
        HitEffect_1,
        RechargeWeapon_1,
        //UI
        UIClick,
        EnergyEmpty
    }
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")] 
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource loopingSeSource;

        [Header("SE Pool Settings")] 
        [SerializeField] private GameObject seSourcePrefab;
        [SerializeField] private int sePoolSize = 10;
        
        [Header("Audio Clips")]
        [SerializeField] private List<SoundClip> seClips;
        [System.Serializable]
        public class SoundClip
        {
            public SoundType type;
            public AudioClip clip;

            [Range(0f, 1f)] public float volume = 1.0f;

            [Header("Easing Settings")] 
            public float fadeInTime = 0.0f;
            public float fadeOutTime = 0.1f;
        }

        private Dictionary<SoundType, SoundClip> seDictionary;
        private List<AudioSource> seSourcePool;

        private CancellationTokenSource bgmFadeCts;
        private CancellationTokenSource loopingSeFadeCts;

        private void Awake()
        {
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
            
            seDictionary = new Dictionary<SoundType, SoundClip>();
            foreach (var soundClip in seClips)
            {
                seDictionary[soundClip.type] = soundClip;
            }

            seSourcePool = new List<AudioSource>();
            for (int i = 0; i < sePoolSize; i++)
            {
                GameObject sourceObj;
                if (seSourcePrefab != null)
                {
                    sourceObj = Instantiate(seSourcePrefab, transform);
                }
                else
                {
                    sourceObj = new GameObject($"SE_Source_{i}");
                    sourceObj.transform.SetParent(transform);
                    sourceObj.AddComponent<AudioSource>();
                }

                AudioSource source = sourceObj.GetComponent<AudioSource>();
                source.playOnAwake = false;
                sourceObj.SetActive(false);
                seSourcePool.Add(source);
            }
        }

        /// <summary>
        /// BGMを再生する
        /// </summary>
        public void PlayBGM(AudioClip clip, float fadeDuration = 1.0f, bool loop = true)
        {
            if(clip==null) return;

            bgmFadeCts?.Cancel();
            bgmFadeCts = new CancellationTokenSource();
            FadeBGMAsync(clip, fadeDuration, loop, bgmFadeCts.Token).Forget();
        }
        
        
        /// <summary>
        /// SEを再生 
        /// </summary>
        public void PlaySE(SoundType type)
        {
            if (seDictionary.TryGetValue(type, out SoundClip soundClip))
            {
                AudioSource sourceToUse = GetAvailableSeSource();
                if (sourceToUse != null)
                {
                    PlayWithFadeAsync(sourceToUse, soundClip).Forget();
                }
            }
            else
            {
                Debug.LogWarning($"SoundType: {type} not found in dictionary.");
            }
        }

        public void PlayLoopingSE(SoundType type)
        {
            if(loopingSeSource == null) return;

            if (seDictionary.TryGetValue(type, out SoundClip soundClip))
            {
                loopingSeFadeCts?.Cancel();
                loopingSeFadeCts = new CancellationTokenSource();
                FadeInLoopingSEAsync(soundClip, loopingSeFadeCts.Token).Forget();
            }
            else
            {
                Debug.LogWarning($"SoundType: {type} not found in dictionary.");
            }
        }

        public void StopLoopingSE()
        {
            if(loopingSeSource == null || !loopingSeSource.isPlaying) return;
            
            loopingSeFadeCts?.Cancel();
            loopingSeFadeCts = new CancellationTokenSource();
            FadeOutLoopingSEAsync(loopingSeFadeCts.Token).Forget();
        }
        
        /* ---- Private Methods---- */

        private AudioSource GetAvailableSeSource()
        {
            foreach (var source in seSourcePool)
            {
                if (!source.gameObject.activeSelf)
                {
                    return source;
                }
            }
            Debug.LogWarning("SE Pool is full. Consider increasing the pool size.");
            return null;
        }
        
        private async UniTaskVoid PlayWithFadeAsync(AudioSource source, SoundClip soundClip)
        {
            var token = source.GetCancellationTokenOnDestroy();
            source.gameObject.SetActive(true);
            
            source.clip = soundClip.clip;
            float targetVolume = soundClip.volume;
            
            if (soundClip.fadeInTime > 0)
            {
                float timer = 0;
                source.volume = 0;
                source.Play();
                while (timer < soundClip.fadeInTime)
                {
                    source.volume = Mathf.Lerp(0, targetVolume, timer / soundClip.fadeInTime);
                    timer += Time.deltaTime;
                    await UniTask.Yield(token);
                }
            }
            
            source.volume = targetVolume;
            if (!source.isPlaying)
            {
                source.Play();
            }

            float waitTime = soundClip.clip.length - soundClip.fadeOutTime;
            if (waitTime > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);
            }

            if (soundClip.fadeOutTime > 0)
            {
                float timer = 0;
                float startVolume = source.volume;
                while (timer < soundClip.fadeOutTime)
                {
                    source.volume = Mathf.Lerp(startVolume, 0, timer / soundClip.fadeOutTime);
                    timer += Time.deltaTime;
                    await UniTask.Yield(token);
                }
            }

            source.Stop();
            source.gameObject.SetActive(false);
        }
        
        
        private async UniTaskVoid FadeBGMAsync(AudioClip newClip, float fadeDuration, bool loop,
            CancellationToken token)
        {
            if (bgmSource.isPlaying)
            {
                float startVolume = bgmSource.volume;
                float timer = 0;
                while ( timer < fadeDuration)
                {
                    bgmSource.volume = Mathf.Lerp(startVolume, 0, timer / fadeDuration);
                    timer += Time.deltaTime;
                    await UniTask.Yield(cancellationToken: token);
                }
            }
            
            bgmSource.Stop();
            bgmSource.clip = newClip;
            bgmSource.loop = loop;
            bgmSource.Play();

            float endVolume = 1.0f;
            float fadeInTimer = 0f;
            while (fadeInTimer < fadeDuration)
            {
                bgmSource.volume = Mathf.Lerp(0, endVolume, fadeInTimer / fadeDuration);
                fadeInTimer += Time.deltaTime;
                await UniTask.Yield(cancellationToken: token);
            }
            bgmSource.volume = endVolume;
        }

        
        private async UniTaskVoid FadeInLoopingSEAsync(SoundClip soundClip, CancellationToken token)
        {
            loopingSeSource.clip = soundClip.clip;
            loopingSeSource.loop = true;
            loopingSeSource.Play();

            float timer = 0f;
            float targetVolume = soundClip.volume;
            while ( timer < soundClip.fadeInTime)
            {
                loopingSeSource.volume = Mathf.Lerp(0, targetVolume, timer / soundClip.fadeInTime);
                timer += Time.deltaTime;
                await UniTask.Yield(cancellationToken: token);
            }
            loopingSeSource.volume = targetVolume;
        }
        
        
        private async UniTaskVoid FadeOutLoopingSEAsync(CancellationToken token)
        {
            SoundClip currentClipData = null;
            foreach(var sc in seDictionary.Values)
            {
                if (sc.clip == loopingSeSource.clip)
                {
                    currentClipData = sc;
                    break;
                }
            }

            if (currentClipData == null || currentClipData.fadeOutTime <= 0)
            {
                loopingSeSource.Stop();
                loopingSeSource.clip = null;
                return;
            }

            float timer = 0;
            float startVolume = loopingSeSource.volume;
            while (timer < currentClipData.fadeOutTime)
            {
                loopingSeSource.volume = Mathf.Lerp(startVolume, 0, timer / currentClipData.fadeOutTime);
                timer += Time.deltaTime;
                await UniTask.Yield(cancellationToken: token);
            }

            loopingSeSource.Stop();
            loopingSeSource.clip = null;
        }
    }    
}

