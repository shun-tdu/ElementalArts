using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemyNum;
    [SerializeField] public float gameScore = 0.0f;
    
    private float playerHpManaged;
    private float absorbedEnergyManaged;

    private GameObject canvas;
    private TextManager textManager;
    private GameObject playerObj;
    // private Player.Player playerScript;
    
    
    void Start()
    {
        if (GameObject.Find("Canvas"))
        {
            canvas = GameObject.Find("Canvas");   
            textManager = canvas.GetComponent<TextManager>();
        }
        if (GameObject.Find("Player"))
        {
            playerObj = GameObject.Find("Player");   
            // playerScript = playerObj.GetComponent<Player.Player>();
        }
        
        //HPをマネージャーで管理するために取得
        // playerHpManaged = playerScript.GetPlayerHp();
        
        ReducePlayerLife(0);
        
        StartCoroutine(GanerateEnemy());
    }
    
    
    //Score
    public void AddScore(float score)
    {
        gameScore += score;
        //Debug.Log(gameScore);
        textManager.ChangeScore(gameScore);
    }
    
    //PlayerHp
    public void ReducePlayerLife(float takenDamage)
    {
        playerHpManaged -= takenDamage;
        textManager.ChangePlayerLife(playerHpManaged);
    }
    
    //AbsorbedEnergy
    public void AddAbsorbedEnergy(float absorbedEnergy)
    {
        absorbedEnergyManaged = absorbedEnergy;
        textManager.ChangeAbsorbedEnergy(absorbedEnergyManaged);
    }

    //ウェーブの実装
    
    //敵生成コルーチン
    IEnumerator GanerateEnemy()
    {
        for (int i = 0; i < enemyNum; i++)
        {
            GameObject obj = Instantiate(enemyPrefab,transform.position+Vector3.right*Random.Range(-10.0f,10.0f)+Vector3.forward*Random.Range(100f,500f),transform.rotation);
            //enemy_list.Add(obj);
            yield return new WaitForSeconds(0.2f);
        }
    }
    
    //スコアゲッター
    public float GetGameScore()
    {
        return gameScore;
    }
}
