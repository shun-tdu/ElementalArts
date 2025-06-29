using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFX.ChainLightning
{
    public class ChainLightningShoot:MonoBehaviour
    {
        [SerializeField] private float refreshRate = 0.01f;
        [SerializeField] private Transform playerFirePoint;
        [SerializeField] private EnemyDetector playerEnemyDetector;
        [SerializeField] private GameObject lineRendererPrefab;
        
        
        private bool shooting = false;
        private bool shot = false;
        private List<GameObject> spawnedLineRenderers = new List<GameObject>();
        
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("F pressed");
                if (playerEnemyDetector.GetEnemiesInRange().Count > 0)
                {
                    if (!shooting)
                    {
                        StartShooting();
                    }
                }
                else
                {
                    StopShooting();
                }
            }

            if (Input.GetKeyUp(KeyCode.F))
            {
                StopShooting();
            }
        }

        
        private void StartShooting()
        {
            Debug.Log("Shoot Render");
            shooting = true;

            if (playerEnemyDetector != null && playerFirePoint != null && lineRendererPrefab != null)
            {
                if (!shot)
                {
                    shot = true;
                    
                    NewLineRenderer(playerFirePoint,playerEnemyDetector.GetClosesEnemy().transform);
                }
            }
        }

        
        private void NewLineRenderer(Transform startPos, Transform endPos)
        {
            GameObject lineR = Instantiate(lineRendererPrefab);
            spawnedLineRenderers.Add(lineR);
            StartCoroutine(UpdateLineRenderer(lineR,startPos,endPos));
        }

        IEnumerator UpdateLineRenderer(GameObject lineR, Transform startPos, Transform endPos)
        {
            if (shooting && shot && lineR != null)
            {
                lineR.GetComponent<LineRendererController>().SetPosition(startPos, endPos);

                yield return new WaitForSeconds(refreshRate);

                StartCoroutine(UpdateLineRenderer(lineR, startPos, playerEnemyDetector.GetClosesEnemy().transform));
            }
        }
        
        
        private void StopShooting()
        {
            shooting = false;
            shot = false;

            for (int i = 0; i < spawnedLineRenderers.Count; i++)
            {
                Destroy(spawnedLineRenderers[i]);
            }
            spawnedLineRenderers.Clear();
        }
    }
}