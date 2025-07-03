using System.Collections.Generic;
using UnityEngine;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Core
{
    [CreateAssetMenu(fileName = "AIPersonality_",menuName = "AI/Personality (Transition-Driven)")]
    public class AIPersonality : ScriptableObject
    {
        [Header("AIの基本パラメータ")] 
        public float visionRange = 50f;
        public float longRangeAttackRange = 300f;
        public float attackRange = 5;
        
        [Header("遷移ルール(上が優先)")]
        public List<Transition> transitions;    //遷移ルール
    }
}