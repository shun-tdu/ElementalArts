using System;
using UnityEngine;

namespace Utility
{
    public class DestructionNotifier:MonoBehaviour
    {
        public event Action OnDestroyed;
        private void OnDestroy() => OnDestroyed?.Invoke();
    }
}