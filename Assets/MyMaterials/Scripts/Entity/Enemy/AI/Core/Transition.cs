using System;
using MyMaterials.Scripts.Entity.Enemy.AI.Core.State;
using MyMaterials.Scripts.Entity.Enemy.AI.Core.Conditions;
using UnityEngine;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Core
{
    [System.Serializable]
    public class Transition
    {
        // インスペクター設定用フィールド
        [SerializeField] 
        private InterfaceReference <IStateCondition> _condition;   //この遷移が発生する条件
        [SerializeField]
        private InterfaceReference<IEnemyState>  _targetState;    //遷移先の状態
        
        public IStateCondition Condition { get; private set; }
        public IEnemyState TargetState { get; private set; }
        
        
        /// <summary>
        /// コードから動的に生成するためのコンストラクタ
        /// </summary>
        public Transition(IStateCondition condition, IEnemyState targetState)
        {
            Condition = condition;
            TargetState = targetState;
        }
        
        
        /// <summary>
        /// インスペクターで設定された値をプロパティに反映させるための初期化メソッド
        /// </summary>
        public void Initialize()
        {
            if (Condition == null) Condition = _condition.Current;
            if(TargetState == null) TargetState = _targetState.Current;
        }
    }
}