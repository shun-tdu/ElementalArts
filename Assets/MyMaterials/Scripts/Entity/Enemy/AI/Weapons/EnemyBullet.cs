using System;
using UnityEngine;

namespace MyMaterials.Scripts.Entity.Enemy.AI.Weapons
{
    public class EnemyBullet:MonoBehaviour
    {
        public float damage = 10f;
        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            Destroy(gameObject, 5f);
        }

        private void OnCollisionEnter(Collision other)
        {
            IDamageable damageableObject = other.gameObject.GetComponent<IDamageable>();
            if (damageableObject != null)
            {
                Vector3 hitPosition = other.contacts[0].point;

                Vector3 hitDirection = rb.velocity.normalized;
                damageableObject.TakeDamage(damage, hitPosition, hitDirection);
            }
            Destroy(gameObject);
        }
    }
}