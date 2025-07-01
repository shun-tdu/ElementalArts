using UnityEngine;
using System.Collections.Generic;


namespace Player.Bit
{
    /// <summary>
    /// 単一のビットの、親から見た相対的な位置と回転を定義する構造体
    /// </summary>
    [System.Serializable]
    public struct BitTransformData
    {
        public Vector3 localPosition;
        public Vector3 localEulerAngles;
    }
    
    /// <summary>
    /// 特定の状態における全ビットのフォーメーション情報を保持するScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "BitFormation_",menuName = "Mecha/Bit Formation Data")]
    public class BitFormationData:ScriptableObject
    {
        public List<BitTransformData> bitDataList;
    }
}