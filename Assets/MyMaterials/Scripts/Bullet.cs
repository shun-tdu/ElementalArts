using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletDamage;
    private Vector3 bulletPos;
    
    void Update()
    {
        bulletPos = this.gameObject.transform.position;
        bulletPos.z = bulletPos.z + bulletSpeed * Time.deltaTime;
        this.gameObject.transform.position = bulletPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Player.Health>().TakeDamage(bulletDamage);
            Destroy(this.gameObject);
        }
    }
}
