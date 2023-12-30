using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Train
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Locomotive : MonoBehaviour
    {
        #region InspectorEditableVars
       
        [SerializeField,Range(10f,500f)] private float speed =280f;
        [SerializeField,Range(0f,10f)] private float turnSpeed = 0.05f;
        [SerializeField, Range(1, 50)] private int offsetPoints;
        [SerializeField, Range(1, 20)] private int foodNeeded;
        [SerializeField] private List<GameObject> wagonPrefabs;
        [SerializeField] private bool randomWagons = true;
        
        #endregion

        #region StableVars
        
        private readonly List<WagonRef> _trainWagons = new ();
        private readonly TrailManager _myTrailManager = new ();
        private const int InAdvancePlanedWagons = 10;
        
        private Rigidbody2D _myRigidbody2D;
        private Camera _mainCam;
        private GameObject _trainParent;
        
        #endregion

        #region NormalVars
        
        private int _nextWagonPointIndex;
        private float _timeSinceLastFixedUpdate;
        private int currentCollectedFood;
        
        #endregion

        #region Events

        public Action OnGameOver,OnWagonAdded;

        #endregion
        
        #region Awake_Start
        private void Awake()
        {
            _myRigidbody2D = GetComponent<Rigidbody2D>();
        }
        private void Start()
        {
            _mainCam = Camera.main;
            UpdateNextWagonIndex();
            _trainParent = new GameObject("Train");
            transform.parent = _trainParent.transform;
        }
        #endregion

        #region Update
        private void Update()
        {
            LerpWagons();
        }

        private void LerpWagons()
        {
            _timeSinceLastFixedUpdate += Time.deltaTime;
            float delta = _timeSinceLastFixedUpdate / Time.fixedDeltaTime;
            delta = Mathf.Clamp(delta, 0f, 1f);
            foreach (WagonRef trainWagonRef in _trainWagons)
            {
                trainWagonRef.Transform.position = Vector3.Lerp(trainWagonRef.PreviousTrailPoint.WorldPosition,
                    trainWagonRef.NextTrailPoint.WorldPosition, delta);
                trainWagonRef.Transform.rotation = Quaternion.Lerp(trainWagonRef.PreviousTrailPoint.WorldRotation,
                    trainWagonRef.NextTrailPoint.WorldRotation, delta);
            }
        }
        #endregion

        #region FixedUpdate
        private void FixedUpdate()
        {
            _timeSinceLastFixedUpdate = 0;
            UpdateTrail();
            UpdateLocomotive();
            UpdateWagons();
        }
        private void UpdateTrail()
        {
            _myTrailManager.AddTrailPoint(new TrailPoint(transform));
        }
        private void UpdateWagons()
        {
            foreach (WagonRef trainWagonRef in _trainWagons)
            {
                if (!_myTrailManager.GetTrailPoint(trainWagonRef.TrialPointIndex, out var point)) continue;
                trainWagonRef.PreviousTrailPoint = trainWagonRef.NextTrailPoint;
                trainWagonRef.NextTrailPoint = point;
            }
        }

        private void UpdateLocomotive()
        {
            Transform myTransform = transform;
            //calculate vector to Cursor
            Vector3 mousePosition = _mainCam.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            Vector2 direction = (mousePosition - myTransform.position).normalized;
            //calculate target rotation
            float targetZ = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            if (Mathf.Abs(targetZ - myTransform.rotation.eulerAngles.z) > 0.05f)
            {
                myTransform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, targetZ),
                    turnSpeed * Time.fixedDeltaTime);
            }

            //update velocity
            _myRigidbody2D.velocity = myTransform.right * (speed * Time.fixedDeltaTime);
        }
        #endregion

        #region Collision

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Food"))
            {
                Destroy(other.gameObject);
                currentCollectedFood++;
                if (currentCollectedFood >= foodNeeded)
                {
                  AddWagon();
                  currentCollectedFood = 0;
                }
            }

            if (other.gameObject.CompareTag("Obstacle"))
            {
                Debug.Log($"Hit YourSelf You lost");
                GameOver();
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Obstacle"))
            {
                Debug.Log($"Hit the border You lost");
                GameOver();
            }
        }

        #endregion

        #region Helper

        private void GameOver()
        {
            OnGameOver?.Invoke();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        private void UpdateNextWagonIndex()
        {
            _nextWagonPointIndex = ((_trainWagons.Count +1)* offsetPoints)-1;
            _myTrailManager.SetTrailLength(_nextWagonPointIndex+1 + offsetPoints *InAdvancePlanedWagons);
        }

        private void AddWagon(int indexID = 0)
        {
            if(indexID < 0 || indexID >= wagonPrefabs.Count) return;
            if (randomWagons) indexID = Mathf.RoundToInt(Random.Range(0f, wagonPrefabs.Count - 1));
            GameObject newWagon = Instantiate(wagonPrefabs[indexID],_trainParent.transform);
            var wagon = new WagonRef(newWagon.transform, _nextWagonPointIndex);
            if(_myTrailManager.GetTrailPoint(_nextWagonPointIndex, out var pointA)) wagon.PreviousTrailPoint = pointA;
            if(_myTrailManager.GetTrailPoint(_nextWagonPointIndex-1, out var pointB)) wagon.NextTrailPoint = pointB;
            _trainWagons.Add(wagon);
            UpdateNextWagonIndex();
            
            OnWagonAdded?.Invoke();
        }
        #endregion
    }
    public class WagonRef
    {
        public WagonRef(Transform transform, int trialPointIndex)
        {
            Transform = transform;
            TrialPointIndex = trialPointIndex;
        }

        public Transform Transform { get; }
        public int TrialPointIndex { get; }

        public TrailPoint NextTrailPoint, PreviousTrailPoint;
    }
}
