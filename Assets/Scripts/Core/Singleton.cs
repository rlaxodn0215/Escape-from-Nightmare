using UnityEngine;

namespace EscapeFromNightmare
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get { return instance; }
        }

        protected virtual bool UseDontDestroyOnLoad
        {
            get { return false; }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;

                if (UseDontDestroyOnLoad)
                {
                    if (transform.parent != null)
                    {
                        transform.SetParent(null);
                    }

                    DontDestroyOnLoad(gameObject);
                }

                return;
            }

            if (instance != (this as T))
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (instance == (this as T))
            {
                instance = null;
            }
        }
    }
}
