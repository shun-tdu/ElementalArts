using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Singletons
{
    public enum SoundType
    {
        // Player Actions
        Step,
        DashStart,
        WeaponFire,
        //Lock-On System
        TargetAcquired,
        IntentionLockOn,
        TargetLost,
        //UI
        UIClick,
    }
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")] 
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource seSource;

        [Header("Audio Clips")]
        [SerializeField] private List<SoundClip> seClips;
        [System.Serializable]
        public class SoundClip
        {
            public SoundType type;
            public AudioClip clip;
        }

        private Dictionary<SoundType, AudioClip> seDictionary;

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
            
            // ListをDictionaryに変換して、再生時に高速にクリップを探索可能にする
            seDictionary = new Dictionary<SoundType, AudioClip>();
            foreach (var soundClip in seClips)
            {
                seDictionary[soundClip.type] = soundClip.clip;
            }
        }
        
        /// <summary>
        /// BGMを再生する
        /// </summary>
        public void PlayBGM(AudioClip clip, bool loop = true)
        {
            if(clip==null) return;

            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
        }
        
        
        /// <summary>
        /// SEを再生 
        /// </summary>
        public void PlaySE(SoundType type)
        {
            if (seDictionary.TryGetValue(type, out AudioClip clip))
            {
                seSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning($"SoundType: {type} not found in dictionary.");
            }
        }
        
        
        /// <summary>
        /// 3D空間の特定の位置でSEを再生
        /// </summary>
        public void PlaySEAtPoint(SoundType type, Vector3 position)
        {
            if (seDictionary.TryGetValue(type, out AudioClip clip))
            {
                AudioSource.PlayClipAtPoint(clip, position);
            }
            else
            {
                Debug.LogWarning($"SoundType: {type} not found in dictionary.");
            }
        }
    }    
}

