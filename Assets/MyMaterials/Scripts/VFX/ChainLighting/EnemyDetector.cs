using System;
using System.Collections.Generic;
using UnityEngine;

namespace VFX.ChainLightning
{
    public class EnemyDetector:MonoBehaviour
    {
        private List<GameObject> enemiesInRange = new List<GameObject>();
        
        /// <summary>
        /// 最も近いエネミーを取得する
        /// </summary>
        /// <returns></returns>
        public GameObject GetClosesEnemy()
        {
            if (enemiesInRange.Count > 0)
            {
                GameObject bestTarget = null;
                float closestDistanceSqr = Mathf.Infinity;
                Vector3 currentPosition = transform.position;

                foreach (GameObject closestEnemy in enemiesInRange)
                {
                    Vector3 directionToTarget = closestEnemy.transform.position - currentPosition;
                    float dSqrToTarget = directionToTarget.sqrMagnitude;

                    if (dSqrToTarget < closestDistanceSqr)
                    {
                        closestDistanceSqr = dSqrToTarget;
                        bestTarget = closestEnemy;
                    }
                }

                return bestTarget;
            }
            else
            {
                return null;
            }
        }

        public List<GameObject> GetEnemiesInRange()
        {
            return enemiesInRange;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                if (enemiesInRange.Count == 0)
                {
                    enemiesInRange.Add(other.gameObject);
                }else if (!enemiesInRange.Contains(other.gameObject))
                {
                    enemiesInRange.Add(other.gameObject);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                if (enemiesInRange.Count > 0)
                {
                    enemiesInRange.Remove(other.gameObject);
                }
            }
        }
    }
}