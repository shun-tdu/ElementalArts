using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace weapon
{
    public interface IWeaponBehavior
    {
        /// <summary>
        /// ボタンを押した瞬間の処理
        /// </summary>
        void OnTriggerDown(Transform muzzle, Transform target);
    
        /// <summary>
        /// ボタンを離したときの処理
        /// </summary>
        void OnTriggerUp(Transform muzzle, Transform target);
    
        /// <summary>
        /// 長押し中毎フレーム呼ばれる処理
        /// </summary>
        void OnTriggerHold(Transform muzzle, Transform target);
    }
}
