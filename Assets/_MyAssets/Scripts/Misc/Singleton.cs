using System;
using UnityEngine;

namespace DashSync.Misc
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        [field: SerializeField]
        public bool isPersistent { get; private set; } = false;

        private static T instance = null;
        public static T Instance
        {
            get
            {
                if(instance == null)
                    instance = FindFirstObjectByType<T>();
                if(instance == null)
                    instance = new GameObject(typeof(T).Name).AddComponent<T>();
                return instance;
            }
        }

        public virtual void Awake()
        {
            if(instance != null)
            {
                if (instance != this as T)
                    Destroy(gameObject);
            }
            else
            {
                instance = this as T;
                if (isPersistent)
                    DontDestroyOnLoad(instance.gameObject);
            }
        }
    }
}