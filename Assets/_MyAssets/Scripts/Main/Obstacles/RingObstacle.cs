using System;
using UnityEngine;

namespace DashSync.Main
{
    [RequireComponent(typeof(LineRenderer), typeof(ObstacleLOD), typeof(Rigidbody))]
    public class RingObstacle : MonoBehaviour, IObstacle
    {
        [SerializeField, Range(0.1f, 10f)]
        private float ringRadius;
        [SerializeField, Range(180f, 360f)]
        private float maxAngle;
        [field: SerializeField]
        public float offsetAngle { get; private set; }
        [SerializeField]
        private float obstacleSpeed;
        [SerializeField]
        private float rotationSpeed;
        [field: SerializeField]
        public LineRenderer line { get; private set; }
        private ObstacleLOD lodDetails;
        private MeshCollider ringCollider;
        [field: SerializeField]
        public Rigidbody thisRigidbody;

        public Action<IObstacle> OnEndReached;

        private void Awake()
        {
            line = GetComponent<LineRenderer>();
            lodDetails = GetComponent<ObstacleLOD>();
            ringCollider = GetComponent<MeshCollider>();
            thisRigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            lodDetails.OnLODAssigned += DrawObstacle;
            lodDetails.OnReachedEnd += EndReached;
        }

        private void OnDisable()
        {
            lodDetails.OnLODAssigned -= DrawObstacle;
            lodDetails.OnReachedEnd -= EndReached;
        }

        void FixedUpdate()
        {
            Vector3 position = thisRigidbody.position
                            + Vector3.back * obstacleSpeed * Time.fixedDeltaTime;
            thisRigidbody.MovePosition(position);

            Vector3 rotation = transform.eulerAngles
                + Vector3.back * rotationSpeed * Time.fixedDeltaTime;
            thisRigidbody.MoveRotation(Quaternion.Euler(rotation));
        }

        public void SetOffsetAngle(float angle)
        {
            offsetAngle = angle;
        }

        public void SetObstacleSpeed(float speed)
        {
            obstacleSpeed = speed;
        }

        public void SetRotationSpeed(float rSpeed)
        {
            rotationSpeed = rSpeed;
        }

        public void DrawObstacle(LODinfo info)
        {
            if (info == null) return;

            line.startWidth = info.width;
            line.endWidth = info.width;
            line.positionCount = info.resolution;
            var deltaAngle = maxAngle / info.resolution;
            for(int i=0; i< info.resolution; i++)
            {
                //Calculate position of point around the ring and apply across z position
                var xPos = ringRadius * Mathf.Cos(((i * deltaAngle) + offsetAngle) * Mathf.Deg2Rad);
                var yPos = ringRadius * Mathf.Sin(((i * deltaAngle) + offsetAngle) * Mathf.Deg2Rad);
                var point = new Vector3(xPos, yPos, 0);
                line.SetPosition(i, point);
            }
            var mesh = new Mesh();
            line.BakeMesh(mesh);
            ringCollider.sharedMesh = mesh;
        }

        public void SetColor(Color color)
        {
            line.material.SetColor("_BaseColor", color);
            line.material.SetColor("_EmissionColor", color * 2f);
        }

        private void EndReached()
        {
            OnEndReached?.Invoke(this);
        }
    }
}