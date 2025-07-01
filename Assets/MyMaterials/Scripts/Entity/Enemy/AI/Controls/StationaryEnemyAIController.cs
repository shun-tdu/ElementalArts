using System;
using UnityEngine;
using MyMaterials.Scripts.Entity.Enemy.AI.Movement;
using MyMaterials.Scripts.Entity.Enemy.AI.Weapons;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Controls
{
    public class StationaryEnemyAIController : MonoBehaviour
    {
        public EnemyVision Vision { get; private set; }
        public EnemyWeapons Weapons { get; private set; }
        public EnemyHealth Health { get; private set; }
        public Transform PlayerTarget { get;  set; }
       
        // ---- ステートマシン関連 ----
        private IEnemyState currentState;
        public readonly IdleState IdleState = new IdleState();
        public readonly SearchState SearchState = new SearchState();
        public readonly AttackState AttackState = new AttackState();

        private void Awake()
        {
            // 必要なコンポーネントを取得
            Vision = GetComponent<EnemyVision>();
            Weapons = GetComponent<EnemyWeapons>();
            Health = GetComponent<EnemyHealth>();
        }

        private void Start()
        {
            if (PlayerTarget == null)
            {
                PlayerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
            }
            ChangeState(IdleState);
        }

        private void Update()
        {
            currentState?.OnUpdate(this);
        }
        
        /// <summary>
        /// AI状態の切り替え処理
        /// </summary>
        public void ChangeState(IEnemyState newState)
        {
            currentState?.OnExit(this);
            currentState = newState;
            currentState.OnEnter(this);
        }
    }
}