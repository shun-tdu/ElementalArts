using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyMaterials.Scripts.Weapon
{
    public interface IWeaponBehavior
    {
        /// <summary>
        /// ボタンを押した瞬間の処理
        /// </summary>
        void OnTriggerDown(WeaponSystem owner,　Transform muzzle, Transform target);
    
        /// <summary>
        /// ボタンを離したときの処理
        /// </summary>
        void OnTriggerUp(WeaponSystem owner,　Transform muzzle, Transform target);
    
        /// <summary>
        /// 長押し中毎フレーム呼ばれる処理
        /// </summary>
        void OnTriggerHold(WeaponSystem owner,　Transform muzzle, Transform target);
    }
}
