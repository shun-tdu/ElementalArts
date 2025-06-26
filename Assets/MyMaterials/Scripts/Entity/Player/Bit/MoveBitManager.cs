using System;
using UnityEngine;

namespace Player.Bit
{
    public class MoveBitManager:MonoBehaviour
    {

        [Header("Setting For MoveBit")]
        [SerializeField] private GameObject boosterBit;
        [SerializeField] private float radius = 1f;

        private Rigidbody playerRigidbody;
        
        private static readonly Vector3[] unitVertices = new Vector3[]
        {
            Vector3.right,
            Vector3.left,
            Vector3.up,
            Vector3.down,
            Vector3.forward,
            Vector3.back
        };

        private void Awake()
        {
            playerRigidbody = GameObject.Find("Player").GetComponent<Rigidbody>();
        }

        private void Start()
        {
            PlaceMoveBit();   
        }
        
        /// <summary>
        /// BoosterBitをPlayerを中心とする正八面体の頂点に配置
        /// </summary>
        private void PlaceMoveBit()
        {
            for (int i = 0; i < unitVertices.Length; i++)
            {
                Vector3 dir = unitVertices[i].normalized;
                Vector3 localPos = dir * radius;
                GameObject go = Instantiate(boosterBit, transform);
                go.name = $"BoosterBit_{i}";
                go.transform.localPosition = localPos;
                go.transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir);
            }
        }
    }
}