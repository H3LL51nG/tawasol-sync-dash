using System.Collections;
using UnityEngine;

namespace DashSync.Networking
{
    public class ProxyNetworkManager : MonoBehaviour
    {
        [field: SerializeField]
        public bool isPlayer { get; private set; }
        [SerializeField, Range(0f, 10f)]
        private float timeBetweenUpdate;
        [SerializeField, Range(0f, 5000f), Header("Lag in ms")]
        private float receiverDelay;
        [SerializeField]
        private float smoothTime;

        private TransformData receivedData;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StartCoroutine(SendOrRecieveNetworkPackets());
        }

        private void Update()
        {
            if (!isPlayer && receivedData != null)
            {
                transform.position = Vector3.Lerp(transform.position, receivedData.position, smoothTime * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, receivedData.rotation, smoothTime * Time.deltaTime);
            }
        }

        IEnumerator SendOrRecieveNetworkPackets()
        {
            while (true)
            {
                if (isPlayer)
                {
                    ProxyNetworkBridge.Instance.AddToBuffer(new TransformData(transform.position, transform.rotation));
                    yield return new WaitForSeconds(timeBetweenUpdate);
                }
                else
                {
                    var waitTime = Mathf.Max(receiverDelay / 1000f, timeBetweenUpdate);
                    yield return new WaitForSeconds(waitTime);
                    receivedData = ProxyNetworkBridge.Instance.ReadFromBuffer(); 
                }
            }
        } 
    }
}