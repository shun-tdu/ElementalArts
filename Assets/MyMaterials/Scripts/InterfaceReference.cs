using System;
using UnityEngine;

// 修正点：Utils という名前空間で全体を囲む


[Serializable]
public class InterfaceReference<Interface> where Interface : class
{
    [SerializeField] private UnityEngine.Object _reference;

    public Interface Current
    {
        get
        {
            if (_reference == null) return null;
            if (_reference is Interface value) return value;

            Debug.LogError($"参照されているオブジェクト({_reference.name})は、'{typeof(Interface)}'のインターフェイスを実装していません。");
            return null;
        }
    }
}