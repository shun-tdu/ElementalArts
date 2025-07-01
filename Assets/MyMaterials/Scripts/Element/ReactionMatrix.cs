using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyMaterials.Scripts.Element
{
    /// <summary>
    /// エレメント反応のルールを持つクラス
    /// </summary>
    [Serializable]
    public class ReactionRule
    {
        public ElementType typeA;
        public ElementType typeB;
        public GameObject resultEffectPrefab;
    }
    
    /// <summary>
    /// エレメント反応の共通処理
    /// </summary>
    [CreateAssetMenu(menuName = "Element/ReactionMatrix")]
    public class ReactionMatrix : ScriptableObject
    {
        public List<ReactionRule> rules;

        public GameObject GetReactionEffect(ElementType type1, ElementType type2)
        {
            foreach (var rule in rules)
            {
                if ((rule.typeA == type1 && rule.typeB == type2) ||
                    (rule.typeA == type2 && rule.typeB == type1))
                {
                    return rule.resultEffectPrefab;
                }
            }

            return null;
        }
    }
}