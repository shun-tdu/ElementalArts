using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private float enemyHp = 100.0f;
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private float speed = 30.0f;
    [SerializeField] private GameObject bullet_enemy;
    [SerializeField] private int attackNum = 5;
    [SerializeField] private float attackDulation = 0.2f;
    [SerializeField] private float score = 0;
    [SerializeField] private Vector3 explosionEffectOffset;

    private Rigidbody myRigid;
    private Vector3 playerPos;
    private GameObject eventManagerObj;
    private EventManager eventManager;


    private void Start()
    {
        if (GameObject.Find("EventManager"))
        {
            eventManagerObj = GameObject.Find("EventManager");
            eventManager = eventManagerObj.GetComponent<EventManager>();
        }

        Move();
        StartCoroutine(ShootSequence());
    }

    public void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        enemyHp -= damage;
        if (enemyHp <= 0)
        {
            Killme();
        }
    }

    //破壊処理
    void Killme()
    {
        eventManager.AddScore(score);
        StartCoroutine(Explode());
    }

    void Move()
    {
        myRigid = this.gameObject.GetComponent<Rigidbody>();
        myRigid.velocity = Vector3.back * speed;
    }

    //攻撃
    void ShotForward()
    {
        Instantiate(bullet_enemy, this.gameObject.transform.position, Quaternion.identity);
    }

    //攻撃コルーチン
    IEnumerator ShootSequence()
    {
        for (int i = 0; i < attackNum; i++)
        {
            ShotForward();
            yield return new WaitForSeconds(attackDulation);
        }
    }

    //破壊時アニメーション
    IEnumerator Explode()
    {
        GameObject explodeEffect = Instantiate(explosionEffectPrefab, this.transform.position + explosionEffectOffset,
            Quaternion.identity);

        yield return new WaitForSeconds(0.2f);

        Destroy(explodeEffect);

        Destroy(this.gameObject);
    }
}