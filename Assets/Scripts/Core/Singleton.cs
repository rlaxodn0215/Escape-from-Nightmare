// -----------------------------------------------------------------------------
// Codex comment pass: Singleton
// Role: Defines shared runtime states, start modes, and base infrastructure used across the project.
// Scope: This script belongs to Core\Singleton.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;

namespace EscapeFromNightmare
{
    // Generic MonoBehaviour singleton base used by project managers that need one active scene instance.
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

        // Caches required component references and prepares this object before other startup code runs.
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

        // Clears static references and event links when this object leaves the scene.
        protected virtual void OnDestroy()
        {
            if (instance == (this as T))
            {
                instance = null;
            }
        }
    }
}
