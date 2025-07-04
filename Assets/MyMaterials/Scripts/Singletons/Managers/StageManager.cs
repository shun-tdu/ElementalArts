using System;
using UnityEngine;
using MyMaterials.Scripts.Singletons;

namespace MyMaterials.Scripts.Singletons.Managers
{
    public class StageManager:MonoBehaviour
    {
        [Header("ステージ設定")] 
        [SerializeField] private AudioClip stageBgm;
        [SerializeField] private float bgmFadeInTime = 2.0f;

        private void Start()
        {
            if (stageBgm != null)
            {
                AudioManager.Instance.PlayBGM(stageBgm, bgmFadeInTime);
            }
        }
    }
}