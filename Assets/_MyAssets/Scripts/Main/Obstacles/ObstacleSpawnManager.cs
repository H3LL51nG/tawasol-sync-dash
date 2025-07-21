using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSync.Main
{
    public class ObstacleSpawnManager : MonoBehaviour
    {
        [field: SerializeField]
        public GameState gameState { get; private set; }
        [SerializeField]
        private float obstacleSpeed;
        [SerializeField]
        private float obstacleMinSpeed;
        [SerializeField]
        private float obstacleMaxSpeed;
        [SerializeField]
        private float rotationSpeed;
        [SerializeField]
        private float rotationMinSpeed;
        [SerializeField]
        private float rotationMaxSpeed;
        [SerializeField]
        private float offsetAnglePerSpawn;
        [SerializeField]
        private float obstacleFrequency;
        [SerializeField]
        private RingObstacle ringObstaclePrefab;
        [SerializeField]
        private HelixObstacle helixObstaclePrefab;
        [SerializeField]
        private Transform ringObstacleParent;
        [SerializeField]
        private Transform helixObstacleParent;
        [SerializeField]
        private Color[] obstacleColors;

        private Queue<IObstacle> inactiveRingObstacles = new Queue<IObstacle>();
        private Queue<IObstacle> inactiveHelixObstacles = new Queue<IObstacle>();
        private List<IObstacle> activeObstacles = new List<IObstacle>();
        private Queue<Color> colorQueue = new Queue<Color>();


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            foreach (var color in obstacleColors)
                colorQueue.Enqueue(color);
            obstacleSpeed = obstacleMinSpeed;
            rotationSpeed = rotationMinSpeed;
            gameState = GameState.PLAY;
            SetUpObstacleQueues();
            StartCoroutine(Spawner());
        }

        private IEnumerator Spawner()
        {
            while(gameState == GameState.PLAY)
            {
                var type = ObstacleType.DOUBLE_HELIX;
                var obstacle = GetNextObstacle(type);

                var randomMultiplier = Random.Range(0, 1) == 0 ? -1 : 1;
                var offset = offsetAnglePerSpawn * randomMultiplier;
                var rotateSpeed = rotationSpeed * randomMultiplier;

                if (type == ObstacleType.RING)
                {
                    if (obstacle[0] is RingObstacle ring)
                    {
                        var prevOffset = activeObstacles.Count == 0 ? 0f : (activeObstacles[activeObstacles.Count - 1] as RingObstacle).offsetAngle;
                        ring.gameObject.SetActive(true);
                        ring.transform.localPosition = Vector3.zero;
                        ring.SetOffsetAngle(prevOffset + offset);
                        ring.SetObstacleSpeed(obstacleSpeed);
                        ring.SetRotationSpeed(rotateSpeed);
                        ring.SetColor(FetchObstacleColor());
                        ring.OnEndReached += ObstacleReachedEnd;
                    }

                    activeObstacles.Add(obstacle[0]);
                }
                else if (type == ObstacleType.HELIX)
                {
                    if (obstacle[0] is HelixObstacle helix)
                    {
                        var prevOffset = activeObstacles.Count == 0 ? 0f : (activeObstacles[activeObstacles.Count - 1] as HelixObstacle).offsetAngle;
                        helix.gameObject.SetActive(true);
                        helix.transform.localPosition = Vector3.zero;
                        helix.SetOffsetAngle(prevOffset + offset);
                        helix.SetObstacleSpeed(obstacleSpeed);
                        helix.SetRotationSpeed(rotateSpeed);
                        helix.SetColor(FetchObstacleColor());
                        helix.OnEndReached += ObstacleReachedEnd;
                    }
                    activeObstacles.Add(obstacle[0]);
                }
                else if (type == ObstacleType.DOUBLE_HELIX)
                {
                    var prevOffset = activeObstacles.Count == 0 ? 0f : (activeObstacles[activeObstacles.Count - 1] as HelixObstacle).offsetAngle;
                    if (obstacle[0] is HelixObstacle helix)
                    {
                        helix.gameObject.SetActive(true);
                        helix.transform.localPosition = Vector3.zero;
                        helix.SetOffsetAngle(prevOffset + offset);
                        helix.SetObstacleSpeed(obstacleSpeed);
                        helix.SetRotationSpeed(rotateSpeed);
                        helix.SetColor(FetchObstacleColor());
                        helix.OnEndReached += ObstacleReachedEnd;
                    }
                    activeObstacles.Add(obstacle[0]);
                    if (obstacle[1] is HelixObstacle helix2)
                    {
                        helix2.gameObject.SetActive(true);
                        helix2.transform.localPosition = Vector3.zero;
                        helix2.SetOffsetAngle(prevOffset + offset + 180);
                        helix2.SetObstacleSpeed(obstacleSpeed);
                        helix2.SetRotationSpeed(rotateSpeed);
                        helix2.SetColor(FetchObstacleColor());
                        helix2.OnEndReached += ObstacleReachedEnd;
                    }
                    activeObstacles.Add(obstacle[1]);
                }
                yield return new WaitForSeconds(1f / obstacleFrequency);
            }
        }

        private void SetUpObstacleQueues()
        {
            inactiveRingObstacles.Clear();
            foreach(Transform child in ringObstacleParent)
            {
                inactiveRingObstacles.Enqueue(child.GetComponent<IObstacle>());
            }
            inactiveHelixObstacles.Clear();
            foreach (Transform child in helixObstacleParent)
            {
                inactiveHelixObstacles.Enqueue(child.GetComponent<IObstacle>());
            }
        }

        private List<IObstacle> GetNextObstacle(ObstacleType type)
        {
            var obstacleList = new List<IObstacle>();
            if(type == ObstacleType.RING)
            {
                if (inactiveRingObstacles.Count == 0)
                {
                    inactiveRingObstacles.Enqueue(Instantiate(ringObstaclePrefab, ringObstacleParent));
                }
                obstacleList.Add(inactiveRingObstacles.Dequeue());
            }
            else if(type == ObstacleType.HELIX)
            {
                if (inactiveHelixObstacles.Count == 0)
                {
                    inactiveHelixObstacles.Enqueue(Instantiate(helixObstaclePrefab, helixObstacleParent));
                }
                obstacleList.Add(inactiveHelixObstacles.Dequeue());
            }
            else if (type == ObstacleType.DOUBLE_HELIX)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (inactiveHelixObstacles.Count == 0)
                    {
                        inactiveHelixObstacles.Enqueue(Instantiate(helixObstaclePrefab, helixObstacleParent));
                    }
                    obstacleList.Add(inactiveHelixObstacles.Dequeue());
                }
            }
            return obstacleList;
        }

        private void ObstacleReachedEnd(IObstacle obstacle)
        {
            if(obstacle is RingObstacle ring)
            {
                ring.gameObject.SetActive(false);
                ring.OnEndReached = null;
                inactiveRingObstacles.Enqueue(ring);
            }
            if (obstacle is HelixObstacle helix)
            {
                helix.gameObject.SetActive(false);
                helix.OnEndReached = null;
                inactiveRingObstacles.Enqueue(helix);
            }
            activeObstacles.Remove(obstacle);
        }

        private Color FetchObstacleColor()
        {
            var color = colorQueue.Dequeue();
            colorQueue.Enqueue(color);
            return color;
        }
    }
}