using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class SpawnBarrierRipples : MonoBehaviour
{
    public GameObject barrierRipples;

    private VisualEffect barrierRipplesVFX;

    private void OnCollisionEnter(Collision other)
    {
        var ripples = Instantiate(barrierRipples, transform) as GameObject;
        barrierRipplesVFX = ripples.GetComponent<VisualEffect>();
        barrierRipplesVFX.SetVector3("SphereCenter",other.contacts[0].point);
            
        Debug.Log("test111");
        
        Destroy(ripples,2);
        /*if (other.gameObject.tag == "EnemyBullet")
        {
            
        }*/
    }
}
