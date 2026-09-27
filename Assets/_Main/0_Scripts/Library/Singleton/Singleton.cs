using UnityEngine;

namespace ScriptLibrary.Singletons
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        // ReSharper disable once InconsistentNaming
        public static T Instance
        { get; private set; }
    
        protected virtual void Awake() 
        {
            if (Instance != null && Instance != this) 
            {
                Destroy(gameObject);
                return;
            } 
            Instance = this as T;
        }
    }
}
