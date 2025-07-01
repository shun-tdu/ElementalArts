using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Bit
{
    public class BitFormationManager:MonoBehaviour
    {

        [Header("参照 (References)")] 
        [SerializeField] private List<Transform> bitModels;

        [Header("追従設定 (Smooth Settings)")] 
        [SerializeField] private float positionSmoothTime = 0.2f;
        [SerializeField] private float rotationSmoothTime = 0.1f;
        
        //内部変数
        private BitFormationData currenTargetFormation;
        private List<Vector3> positionVelocities;
        
        
        private void Awake()
        {
            positionVelocities = new List<Vector3>(new Vector3[bitModels.Count]);
        }

        /// <summary>
        /// PlayerControllerから呼び出され、目標フォーメーションを設定する
        /// </summary>
        public void SetTargetFormation(BitFormationData newFormation)
        {
            if (newFormation != null)
            {
                currenTargetFormation = newFormation;
            }
        }

        private void LateUpdate()
        {
            if(currenTargetFormation == null) return;

            for (int i = 0; i < bitModels.Count; i++)
            {
                if (i >= currenTargetFormation.bitDataList.Count) continue;

                BitTransformData targetData = currenTargetFormation.bitDataList[i];
                Transform bit = bitModels[i];

                Vector3 targetLocalPosition = targetData.localPosition;

                Quaternion targetLocalRotation = Quaternion.Euler(targetData.localEulerAngles);

                Vector3 currentVelocity = positionVelocities[i];
                bit.localPosition =
                    Vector3.SmoothDamp(bit.localPosition, targetLocalPosition, ref currentVelocity, positionSmoothTime);
                positionVelocities[i] = currentVelocity;

                bit.rotation = Quaternion.Slerp(bit.localRotation, targetLocalRotation, Time.deltaTime / rotationSmoothTime);
            }
        }
    }
}