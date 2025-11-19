using UnityEngine;

namespace Assets.Scripts.Managers
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region Properties:
        public static T Instance { get; private set; }
        #endregion

        #region MonoBehaviour Methods:
        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this as T;
        }
        #endregion
    }
}