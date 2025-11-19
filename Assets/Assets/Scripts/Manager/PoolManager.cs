using Assets.Scripts.Gameplay.Core.Pool;
using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class PoolManager : SingletonMonoBehaviour<PoolManager>
    {
        #region Private Variables

        private Dictionary<GameObject, object> _pools = new Dictionary<GameObject, object>();
        private Dictionary<MonoBehaviour, GameObject> _instanceToPrefab = new Dictionary<MonoBehaviour, GameObject>();

        [SerializeField, Tooltip("List of prefabs to manage in the pool.")]
        private List<GameObject> _prefabs = new List<GameObject>();

        [SerializeField, Tooltip("Initial number of objects in each pool.")]
        private int _initialPoolObjects = 10;

        #endregion

        #region Monobehaviour Functions

        private void Start()
        {
            RegisterAllPools();
        }

        #endregion

        #region Private Functions

        private void RegisterAllPools()
        {
            foreach (GameObject prefab in _prefabs)
            {
                if (!_pools.ContainsKey(prefab))
                {
                    _pools[prefab] = new ObjectPool<MonoBehaviour>(prefab, _initialPoolObjects);
                }
            }
        }

        #endregion

        #region Public Functions

        public T Spawn<T>(GameObject prefab, Vector3 position) where T : MonoBehaviour
        {
            if (_pools.TryGetValue(prefab, out object poolObj))
            {
                ObjectPool<MonoBehaviour> pool = poolObj as ObjectPool<MonoBehaviour>;
                T obj = pool.Get().GetComponent<T>();

                obj.transform.position = position;

                _instanceToPrefab[obj] = prefab;

                return obj;
            }

            Debug.LogWarning($"No pool registered for {prefab.name}");
            return null;
        }

        public void Despawn(MonoBehaviour obj)
        {
            if (_instanceToPrefab.TryGetValue(obj, out GameObject prefab))
            {
                if (_pools.TryGetValue(prefab, out object poolObj))
                {
                    ObjectPool<MonoBehaviour> pool = poolObj as ObjectPool<MonoBehaviour>;
                    pool.ReturnToPool(obj);

                    _instanceToPrefab.Remove(obj);
                }
                else
                {
                    Debug.LogWarning($"No pool registered for {prefab.name}");
                }
            }
            else
            {
                Debug.LogWarning($"The object {obj.name} is not registered in any pool");
            }
        }

        #endregion
    }
}