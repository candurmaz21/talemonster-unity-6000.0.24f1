
using System.Collections.Generic;
using UnityEngine;

namespace Game.PoolSystem
{
    public sealed class GameObjectPool<T> where T : Component
    {
        private readonly T prefab;
        private readonly Transform poolRoot;
        private readonly Stack<T> pool = new Stack<T>();

        public GameObjectPool(T rPrefab, Transform rPoolRoot, int preWarmCount = 0)
        {
            prefab = rPrefab;
            poolRoot = rPoolRoot;

            for (int i = 0; i < preWarmCount; i++)
            {
                var obj = Object.Instantiate(prefab, poolRoot);
                obj.gameObject.SetActive(false);
                pool.Push(obj);
            }
        }

        public T Get()
        {
            T obj;
            if (pool.Count > 0)
            {
                obj = pool.Pop();
                obj.gameObject.SetActive(true);
            }
            else
            {
                obj = Object.Instantiate(prefab, poolRoot);
            }
            return obj;
        }

        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(poolRoot, false);
            pool.Push(obj);
        }
    }
}