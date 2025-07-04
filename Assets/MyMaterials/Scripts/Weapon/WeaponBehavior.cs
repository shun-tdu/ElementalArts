using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyMaterials.Scripts.Weapon
{
    public　abstract class WeaponBehavior : ScriptableObject, IWeaponBehavior
    {
        public abstract void OnTriggerDown(WeaponSystem owner,　Transform muzzle, Transform target);
        public abstract void OnTriggerUp(WeaponSystem owner,　Transform muzzle, Transform target);
        public abstract void OnTriggerHold(WeaponSystem owner,　Transform muzzle, Transform target);
    }    
}
