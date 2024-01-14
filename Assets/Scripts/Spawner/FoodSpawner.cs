using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spawner
{
    public class FoodSpawner : MonoBehaviour
    {
        #region InspectorEditableVars
        
        public bool continueSpawning = true;
        [SerializeField, Range(.1f, 100f)] private float spawnDelay =5f;
        [SerializeField, Range(5f, 100f)] private float maxSpawnDistance = 15f;
        [SerializeField, Range(2f, 50f)] private float minSpawnDistance = 4f;
        [SerializeField] private GameObject objectToSpawn;
        
        #endregion
        private void Start()
        {
            StartCoroutine(SpawnLoop());
        }

        private IEnumerator SpawnLoop()
        {
            while (continueSpawning)
            {
                yield return new WaitForSeconds(spawnDelay);

                Vector3 direction = Random.insideUnitCircle.normalized;

                var scalar = Random.Range(0f, 1f);

                var position = transform.position + direction * Mathf.Lerp(scalar, minSpawnDistance,maxSpawnDistance);

                Instantiate(objectToSpawn, position, Quaternion.Euler(new Vector3(0,0,Random.Range(0f,359f))));
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position,maxSpawnDistance);
            Gizmos.DrawWireSphere(transform.position,minSpawnDistance);
        }
    }
}
