using UnityEngine;
using UnityEngine.UI;
using MyMaterials.Scripts.Managers;
using MyMaterials.Scripts.Managers.Singletons;

namespace MyMaterials.Scripts.Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("ボタンの参照")] 
        [SerializeField] private Button stageSelectButton;
        [SerializeField] private Button customizeButton;
        [SerializeField] private Button helpMenuButton;
        [SerializeField] private Button exitButton;
        
        [Header("サウンド設定")]
        [SerializeField] private SoundType clickSound;
        [SerializeField] private AudioClip titleBgm;

        private void Start()
        {
            // --- ボタンの登録 ---
            stageSelectButton.onClick.AddListener(OnStageSelectGame);
            customizeButton.onClick.AddListener(OnCustomize);
            helpMenuButton.onClick.AddListener(OnHelpMenu);
            exitButton.onClick.AddListener(OnExitGame);

            // --- BGMの再生 ---
            if (titleBgm != null)
            {
                AudioManager.Instance.PlayBGM(titleBgm, 1.0f);
            }
        }

        private void OnDestroy()
        {
            // イベントの購読解除
            stageSelectButton.onClick.RemoveListener(OnStageSelectGame);
            customizeButton.onClick.RemoveListener(OnCustomize);
            helpMenuButton.onClick.RemoveListener(OnHelpMenu);
            exitButton.onClick.RemoveListener(OnExitGame);

        }

        /// <summary>
        /// ゲーム開始ボタンが押されたときの処理
        /// </summary>
        private void OnStageSelectGame()
        {
            PlayClickSound();
        
            // GameManagerに、ゲームプレイ状態への遷移を依頼する
            GameManager.Instance.ChangeState(GameState.Playing);
        }
        
        /// <summary>
        /// カスタマイズボタンが押されたときの処理
        /// </summary>
        private void OnCustomize()
        {
            PlayClickSound();
            Debug.Log("カスタマイズ画面に遷移");
        }

        /// <summary>
        /// ヘルプメニューボタンが押されたときの処理
        /// </summary>
        private void OnHelpMenu()
        {
            PlayClickSound();
            Debug.Log("ヘルプメニューを表示");
        }
        
        /// <summary>
        /// オプションボタンが押されたときの処理
        /// </summary>
        private void OnOptions()
        {
            PlayClickSound();
            Debug.Log("オプション画面を開きます...（未実装）");
            // ここでオプション画面のパネルを表示する処理などを書く
        }

        /// <summary>
        /// ゲーム終了ボタンが押されたときの処理
        /// </summary>
        private void OnExitGame()
        {
            PlayClickSound();
            Debug.Log("ゲームを終了します。");

            
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void PlayClickSound()
        {
            AudioManager.Instance.PlaySE(clickSound);
        }
        
    }
}