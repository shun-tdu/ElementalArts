using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

namespace MyMaterials.Scripts.Singletons
{
    public enum GameState
    {
        Boot,       //起動時
        Title,      //タイトル画面
        Playing,    //プレイ中
        Paused,     //ポーズ中
        StageClear, //ステージクリア
        GameOver    //ゲームオーバー
    }
    public class GameManager : MonoBehaviour
    {
        //シングルトン化
        public static GameManager Instance { get; private set; }
        
        // ---- ゲーム状態 ----
        public GameState CurrentState { get; private set; }
        public event Action<GameState> OnStateChanged;  // 状態変化を通知するイベント
        
        // ---- ゲームデータ ----
        public float PlayTime { get; private set; }
        public int Score { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (SceneManager.GetActiveScene().name == "Boot")
            {
                ChangeState(GameState.Playing);
            }
        }

        private void Update()
        {
            if (CurrentState == GameState.Playing)
            {
                PlayTime += Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                if (CurrentState == GameState.Playing) ChangeState(GameState.Paused);
                else if (CurrentState == GameState.Paused) ChangeState(GameState.Playing);
            }
        }


        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;

            CurrentState = newState;

            switch (newState)
            {
                case GameState.Boot:
                    //初期化処理
                    break;
                case GameState.Title:
                    Time.timeScale = 1f;
                    SceneManager.LoadScene("Title");
                    break;
                case GameState.Playing:
                    Time.timeScale = 1f;
                    if (SceneManager.GetActiveScene().name != "GamePlay")
                    {
                        ResetGameData();
                        SceneManager.LoadScene("GamePlay");
                    }
                    break;
                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;
                case GameState.StageClear:
                    Time.timeScale = 1f;
                    SceneManager.LoadScene("Result");
                    break;
                case GameState.GameOver:
                    Time.timeScale = 1f;
                    SceneManager.LoadScene("Result");
                    break;
            }
            OnStateChanged?.Invoke(newState);
        }
        
        // ゲームデータをリセットする
        private void ResetGameData()
        {
            PlayTime = 0f;
            Score = 0;
        }
    
        public void AddScore(int amount)
        {
            if(CurrentState == GameState.Playing)
            {
                Score += amount;
            }
        }
        
    }    
}

