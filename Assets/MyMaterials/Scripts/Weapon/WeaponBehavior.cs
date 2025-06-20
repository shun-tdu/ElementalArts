using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace weapon
{
    public　abstract class WeaponBehavior : ScriptableObject, IWeaponBehavior
    {
        public abstract void OnTriggerDown(Transform muzzle, Transform target);
        public abstract void OnTriggerUp(Transform muzzle, Transform target);
        public abstract void OnTriggerHold(Transform muzzle, Transform target);
    }    
}
