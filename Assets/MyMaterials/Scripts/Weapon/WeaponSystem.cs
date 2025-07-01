using UnityEngine;

namespace MyMaterials.Scripts.Weapon
{
    public class WeaponSystem : MonoBehaviour
    {
        [Header("Behavior")] [SerializeField] private WeaponBehavior behavior;

        [Header("Hardware")] [SerializeField] private Transform muzzlePoint;

        private Transform currentTarget;

        public void SetLockOnTarget(Transform target)
        {
            this.currentTarget = target;
        }

        /// <summary> 押した瞬間の処理を受け取る </summary>
        public void OnTriggerDown(Transform muzzle)
        {
            if(behavior!=null)
                behavior.OnTriggerDown(muzzlePoint, currentTarget);   
        }

        /// <summary> 長押し中毎フレームの処理を受け取る </summary>
        public void OnTriggerHold(Transform muzzle)
        {
            if(behavior!=null)
                behavior.OnTriggerHold(muzzlePoint, currentTarget);
        }

        /// <summary> 離した瞬間の処理を受け取る </summary>
        public void OnTriggerUp(Transform muzzle)
        {
            if(behavior!=null)
                behavior.OnTriggerUp(muzzlePoint, currentTarget);
        }
    }
}