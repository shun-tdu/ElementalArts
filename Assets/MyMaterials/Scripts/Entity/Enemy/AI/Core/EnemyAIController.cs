using System;
using System.Collections.Generic;
using MyMaterials.Scripts.Entity.Enemy.AI.Core.Conditions;
using UnityEngine;
using MyMaterials.Scripts.Entity.Enemy.AI.Core.State;
using MyMaterials.Scripts.Entity.Enemy.AI.Movement;
using MyMaterials.Scripts.Entity.Enemy.AI.Weapons;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Core
{
    public class EnemyAIController : MonoBehaviour
    {
        [Header("AIの行動指針設定")] 
        [SerializeField] private AIPersonality personality;
        
        // ---- 外部コンポーネント ----
        public EnemyVision Vision { get; private set; }
        public EnemyHealth Health { get; private set; }
       
        // ---- ステートマシン関連 ----
        private IEnemyState currentState;
        private List<Transition> runtimeTransitions;
        private bool isDead = false;

        private void Awake()
        {
            // 必要なコンポーネントを取得
            Vision = GetComponent<EnemyVision>();
            Health = GetComponent<EnemyHealth>();

            InitializeStateMachine();
        }

        
        private void Start()
        {
            // ルールリストの最後にある状態を初期状態にする
            if (runtimeTransitions != null && runtimeTransitions.Count > 0)
            {
                ChangeState(runtimeTransitions[runtimeTransitions.Count - 1].TargetState);
            }
        }

        
        private void Update()
        {
            if(isDead) return;
            if(personality == null || runtimeTransitions == null) return;
            
            // 遷移ルールを上からチェック
            foreach (var transition in runtimeTransitions)
            {
                //条件が満たされた最初のルールが見つかったら、状態を切り替えてループを抜ける
                if (transition.Condition.IsMet(this, personality))
                {
                    if ( currentState == null || currentState.GetType() != transition.TargetState.GetType())
                    {
                        ChangeState(transition.TargetState);
                    }
                    break;
                }
            }
            currentState?.OnUpdate(this);
        }
        
        
        /// <summary>
        /// AI状態の切り替え処理
        /// </summary>
        public void ChangeState(IEnemyState newState)
        {
            if(isDead) return;
            
            currentState?.OnExit(this);
            currentState = newState;
            currentState.OnEnter(this);
        }
        

        public void NotifyDeath()
        {
            isDead = true;
        }

        /// <summary>
        /// AIPersonalityから設定を読み込み、このAIの行動パターンを構築する
        /// </summary>
        private void InitializeStateMachine()
        {
            // Personalityに実装された、ユニークなStateとConditionを全て集める
            var originalStates = new HashSet<IEnemyState>();
            var originalConditions = new HashSet<IStateCondition>();
            foreach (var t in personality.transitions) 
            { 
                //インスペクターの値をプロパティに反映させる
                t.Initialize();
                if (t.TargetState != null) originalStates.Add(t.TargetState);
                if (t.Condition != null) originalConditions.Add(t.Condition);
            }
            
            // 集めたStateとConditionのクローンを生成し、辞書にマッピング
            var stateCloneMap = new Dictionary<IEnemyState, IEnemyState>();
            foreach (var state in originalStates)
            {
                stateCloneMap[state] = Instantiate(state as ScriptableObject) as IEnemyState;
            }

            var conditionCloneMap = new Dictionary<IStateCondition, IStateCondition>();
            foreach (var cond in originalConditions)
            {
                conditionCloneMap[cond] = Instantiate(cond as ScriptableObject) as IStateCondition;
            }
            
            // クローンを使って、このAI専用の遷移ルールリストを構築する
            runtimeTransitions = new List<Transition>();
            foreach (var originalTransition in personality.transitions)
            {
                var clonedCondition = conditionCloneMap[originalTransition.Condition];
                var clonedState = stateCloneMap[originalTransition.TargetState];
                runtimeTransitions.Add(new Transition(clonedCondition, clonedState));
            }
        }
    }
}