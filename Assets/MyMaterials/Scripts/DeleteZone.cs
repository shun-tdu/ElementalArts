using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        string layerName = LayerMask.LayerToName(other.gameObject.layer);

        if (layerName == "DeleteZone")
        {
            return;
        }
        else
        {
            Destroy(other.gameObject);
            //Debug.Log("test");   
        }
    }
}
