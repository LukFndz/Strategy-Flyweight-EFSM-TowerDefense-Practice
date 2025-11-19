using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Core.Pool
{
    public class ObjectPool<T> where T : MonoBehaviour
    {
        private Queue<T> pool = new Queue<T>();
        private GameObject prefab;
        private Transform parent;

        public ObjectPool(GameObject prefab, int initialSize, Transform parent = null)
        {
            this.prefab = prefab;
            this.parent = parent;

            for (int i = 0; i < initialSize; i++)
            {
                T obj = GameObject.Instantiate(prefab, parent).GetComponent<T>();
                obj.gameObject.SetActive(false);
                pool.Enqueue(obj);
            }
        }

        public T Get()
        {
            if (pool.Count > 0)
            {
                T obj = pool.Dequeue();
                obj.gameObject.SetActive(true);
                return obj;
            }

            return GameObject.Instantiate(prefab, parent).GetComponent<T>(); // Instancia correctamente
        }

        public void ReturnToPool(T obj)
        {
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }
}