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
            // other.gameObject.GetComponent<Player.PlayerHealth>().TakeDamage(bulletDamage);
            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(bulletDamage, transform.position, transform.forward);
            }
            Destroy(this.gameObject);
        }
    }
}
