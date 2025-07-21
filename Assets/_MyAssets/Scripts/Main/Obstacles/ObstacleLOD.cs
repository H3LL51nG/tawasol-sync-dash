using System;
using System.Collections.Generic;
using UnityEngine;

namespace DashSync.Main
{
    public class ObstacleLOD : MonoBehaviour
    {
        [SerializeField]
        private LODinfo[] lodInfo;
        private Stack<LODinfo> lodStack = new Stack<LODinfo>();
        private LODinfo currentInfo = null;
        private Camera mainCam;

        public Action<LODinfo> OnLODAssigned = null;
        public Action OnReachedEnd = null;

        private void OnEnable()
        {
            ResetStack();
        }

        private void OnDisable()
        {
            ClearStack();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            mainCam = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            if (currentInfo == null)
                return;

            //Check distance and update LOD
            var distanceFromCamera = Vector3.Distance(mainCam.transform.position, transform.position);
            var delta = (transform.position - mainCam.transform.position).normalized;
            var direction = Vector3.Dot(Vector3.forward, delta);
            distanceFromCamera *= direction > 0 ? 1 : -1;
            if (!currentInfo.isBeyondThreshold(distanceFromCamera))
            {
                if (lodStack.Count > 0)
                {
                    currentInfo = lodStack.Pop();
                    OnLODAssigned?.Invoke(currentInfo);
                }
                else
                {
                    //Reached End of Life
                    OnReachedEnd?.Invoke();
                }
            }
        }

        private void ResetStack()
        {
            if (lodInfo.Length == 0)
                return;

            //Assign LOD details to stack
            lodStack.Clear();
            foreach (var info in lodInfo)
                lodStack.Push(info);

            currentInfo = lodStack.Pop();
            OnLODAssigned?.Invoke(currentInfo);
        }

        private void ClearStack()
        {
            lodStack.Clear();
            currentInfo = null;
        }
    }

    [System.Serializable]
    public class LODinfo
    {
        [field: SerializeField]
        public float distanceThreshold { get; private set; }
        [field: SerializeField]
        public int resolution { get; private set; }
        [field: SerializeField]
        public float width { get; private set; } 

        public bool isBeyondThreshold(float distance)
        {
            return distance > distanceThreshold;
        }
    }
}