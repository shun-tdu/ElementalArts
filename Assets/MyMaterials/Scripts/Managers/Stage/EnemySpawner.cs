using UnityEngine;

namespace MyMaterials.Scripts.Managers.Stage
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("スポーン設定")]
        [Tooltip("このスポナーを中心とした、敵が出現するランダム範囲の半径")]
        [SerializeField] private float spawnRadius = 5.0f;
        
        /// <summary>
        /// 指定された敵プレハブを、このスポナーの位置に生成する
        /// </summary>
        public GameObject Spawn(GameObject enemyPrefab)
        {
            if (enemyPrefab == null) return null;
            
            // スポナーの位置を中心に、半径spawnRadius内のランダムな位置を計算
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            Vector3 spawnPosition = transform.position + randomOffset;
            
            return Instantiate(enemyPrefab, spawnPosition, transform.rotation);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f); // 半透明の緑色
            Gizmos.DrawSphere(transform.position, spawnRadius);
        }
    }
}