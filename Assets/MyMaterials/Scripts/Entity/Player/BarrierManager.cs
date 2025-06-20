using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BarrierManager : MonoBehaviour
{
    [SerializeField] private GameObject barrierRippled;
    [SerializeField] private int animationFPS = 100;
    [SerializeField] private int spawnTime = 3;
    [SerializeField] private int extinguishTime = 2;
    [SerializeField] private float barrierSize = 2.4f;
    
    private const float rotationSpeed = 20.0f;
    
    private VisualEffect barrierVFX;
    private Vector3 barrierAngle = Vector3.zero;

    void Start()
    {
        barrierVFX = this.gameObject.GetComponent<VisualEffect>();
        barrierVFX.SetFloat("BarrierSize", 0.01f);
        //this.gameObject.SetActive(false);
    }

    void Update()
    {
        //barrierVFX.SetVector3("BarrierAngle", barrierAngle);
        RotateBarrier(rotationSpeed);
        if (Input.GetKeyDown(KeyCode.B))
        {
            SpawnBarrier(spawnTime);
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            ExtinguishBarrier(extinguishTime);
        }
    }

    
    IEnumerator SpawnBarrierCoroutine(int spawnTime)
    {
        float timeStep = spawnTime / animationFPS;
        
        for (int i = 1; i <= animationFPS * spawnTime; i++)
        {
            barrierVFX.SetFloat("BarrierSize", barrierSize * i / (animationFPS * spawnTime));
            yield return new WaitForSeconds(1/animationFPS);
        }
    }
    
    IEnumerator ExtinguishBarrierCoroutine(int extinguishTime)
    {
        float timeStep = extinguishTime / animationFPS;
        
        for (int i = animationFPS*extinguishTime; i >= 1; i--)
        {
            barrierVFX.SetFloat("BarrierSize", barrierSize * i / (animationFPS * extinguishTime));
            yield return new WaitForSeconds(1/animationFPS);
        }
    }
    
    //バリアを生成
    void SpawnBarrier(int spawnTime)
    {
        //this.gameObject.SetActive(true);

        StartCoroutine(SpawnBarrierCoroutine(spawnTime));
    }

    //バリアを消去
    void ExtinguishBarrier(int extinguishTime)
    {
        StartCoroutine(ExtinguishBarrierCoroutine(extinguishTime));
        
        //this.gameObject.SetActive(false);
    }

    
    void RotateBarrier(float rotationSpeed)
    {
        Vector3 barrierAngle = barrierVFX.GetVector3("BarrierAngle");
        barrierAngle.y = barrierAngle.y + rotationSpeed * Time.deltaTime;
        barrierVFX.SetVector3("BarrierAngle",barrierAngle);
    }
}