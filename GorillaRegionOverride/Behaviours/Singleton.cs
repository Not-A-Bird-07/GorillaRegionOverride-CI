using UnityEngine;

namespace GorillaRegionOverride.Behaviours
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        public T SelfInstance => gameObject.GetComponent<T>();

        public void Awake()
        {
            if (Instance && Instance != SelfInstance) Destroy(SelfInstance);
            Instance = SelfInstance;
            Initialize();
        }

        protected virtual void Initialize()
        {

        }
    }
}
