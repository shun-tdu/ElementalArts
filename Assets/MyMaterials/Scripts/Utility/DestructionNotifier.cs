using System;
using UnityEngine;

namespace MyMaterials.Scripts.Utility
{
    public class DestructionNotifier:MonoBehaviour
    {
        public event Action OnDestroyed;
        private void OnDestroy() => OnDestroyed?.Invoke();
    }
}