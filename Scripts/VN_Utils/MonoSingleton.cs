using UnityEngine;

namespace KuroNovel.Utils
{
    public class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        private static readonly object _lock = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock) // Ensure thread safety
                    {
                        if (_instance == null) // Double-check locking
                        {
                            _instance = FindObjectOfType<T>();

                            if (_instance != null) return _instance;

                            GameObject gameObject = new GameObject();
                            gameObject.name = typeof(T).Name;
                            _instance = gameObject.AddComponent<T>();
                        }
                    }
                }

                return _instance;
            }
        }

        public virtual void Awake()
        {
            if (_instance == null)
            {
                lock (_lock) // Ensure thread safety during Awake as well
                {
                    if (_instance == null)
                    {
                        _instance = this as T;
                        DontDestroyOnLoad(gameObject);
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}
