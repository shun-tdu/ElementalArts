using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


[CustomPropertyDrawer(typeof(InterfaceReference<>), true)]
public class InterfaceReferenceDrawer : PropertyDrawer
{
    private static float LineHeight { get { return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; } }

    // フィールドの型の取得（配列の場合、その各要素の型を取得）
    protected Type FieldType
    {
        get
        {
            var fieldType = fieldInfo.FieldType;
            if (fieldType.IsArray)
            {
                fieldType = fieldType.GetElementType();
            }
            else if (typeof(IList).IsAssignableFrom(fieldType))
            {
                fieldType = fieldType.GetGenericArguments()[0];
            }
            return fieldType;
        }
    }

    // 型引数で指定した interface の型の獲得
    protected Type InterfaceType => FieldType.GetGenericArguments()[0];

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var referenceProp = property.FindPropertyRelative("_reference");
        // GUI の描画
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.ObjectField(position, referenceProp, label);
        EditorGUI.EndProperty();
        // 入力値の検証
        var reference = referenceProp.objectReferenceValue;
        if (reference is null) return;
        if (reference is GameObject go)
        {
            // GameObject が入力された場合はアタッチされているコンポーネントを検索する
            if (go.TryGetComponent(InterfaceType, out Component component))
            {
                reference = component;
            }
            else
            {
                reference = null;
            }
        }
        if (!IsValid(reference))
        {
            reference = null;
            Debug.LogError($"'{property.displayName}' is able to reference ONLY Object implemented '{InterfaceType}' <at {property.serializedObject.targetObject}>");
        }
        referenceProp.objectReferenceValue = reference;
    }

    // reference オブジェクトが InterfaceType を 継承 / 実装 しているかどうかチェックする関数
    protected virtual bool IsValid(UnityEngine.Object reference)
    {
        if (reference is null) return false;
        var refType = reference.GetType();
        return InterfaceType.IsAssignableFrom(refType);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return LineHeight;
    }
}