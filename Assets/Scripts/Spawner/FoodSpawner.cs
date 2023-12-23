using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spawner
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class FoodSpawner : MonoBehaviour
    {
        #region InspectorEditableVars
        
        public bool continueSpawning = true;
        [SerializeField, Range(.1f, 100f)] private float spawnDelay =5f;
        [SerializeField] private GameObject objectToSpawn;
        
        #endregion

        #region StableVars
        
        private BoxCollider2D _spawnBox;

        #endregion
        
        #region Awake_Start_Validate
        private void OnValidate()
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
        }

        
        private void Awake()
        {
            _spawnBox = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            StartCoroutine(SpawnLoop());
        }
        #endregion

        private IEnumerator SpawnLoop()
        {
            while (continueSpawning)
            {
                yield return new WaitForSeconds(spawnDelay);
                var bounds = _spawnBox.bounds;
                var min = bounds.min;
                var max = bounds.max;
                Vector3 position = new Vector3(
                    Random.Range(min.x, max.x),
                    Random.Range(min.y, max.y),
                    0);

                position += (Vector3)_spawnBox.offset + transform.position;

                Instantiate(objectToSpawn, position, Quaternion.Euler(new Vector3(0,0,Random.Range(0f,359f))));
            }
        }
    }
}
