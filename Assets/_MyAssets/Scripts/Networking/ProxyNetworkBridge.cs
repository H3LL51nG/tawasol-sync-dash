using DashSync.Misc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashSync.Networking
{
    public class ProxyNetworkBridge : Singleton<ProxyNetworkBridge>
    {
        [field: SerializeField]
        public Queue<TransformData> dataBuffer { get; private set; } = new Queue<TransformData>();
        [field: SerializeField]
        public int bufferSize { get; private set; }

        public void AddToBuffer(TransformData data)
        {
            if (dataBuffer.Count < bufferSize)
                dataBuffer.Enqueue(data);
            else
            {
                dataBuffer.Dequeue();
                dataBuffer.Enqueue(data);
            }
        }

        public TransformData ReadFromBuffer()
        {
            return dataBuffer.Dequeue();
        }
    }

    [System.Serializable]
    public class TransformData
    {
        public Vector3 position;
        public Quaternion rotation;

        public TransformData(Vector3 pos, Quaternion rot)
        {
            position = pos;
            rotation = rot;
        }
    }
}