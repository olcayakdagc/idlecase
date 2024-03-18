using System;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPooler : Singleton<ObjectPooler>
{
    [Serializable]
    public class Pool
    {
        [Tooltip("Give a tag to the pool to call")]
        public string Tag;
        [Tooltip("The prefab to be pooled")]
        public GameObject Prefab;
        [Tooltip("The size (count) of the pool")]
        public int InitialSize = 10;
        [Tooltip("The max size (count) pool can reach")]
        public int MaxSize = 10;
    }

    private class Pooled
    {
        public Queue<GameObject> PooledObjectsQueue;
        public List<GameObject> ActiveList = new List<GameObject>();
        public int CountAll;
        public int CountMax;
        public int CountActive => ActiveList.Count;
        public int CountInactive => PooledObjectsQueue.Count;
    }

    [SerializeField] private List<Pool> pools = new List<Pool>();
    private Dictionary<string, Pooled> poolDictionary = new Dictionary<string, Pooled>();

    private void Awake()
    {
        Initialize();
    }

    public ObjectPooler Initialize()
    {
        foreach (var pool in pools)
            AddToPool(pool.Tag, pool.Prefab, pool.InitialSize, pool.MaxSize);

        return this;
    }


    public void DisableAllPooledObjects()
    {
        foreach (var pool in poolDictionary.Values)
        {
            var count = pool.ActiveList.Count;
            for (var i = 0; i < count; i++)
            {
                var go = pool.ActiveList[0];
                go.transform.SetParent(transform);
                go.gameObject.SetActive(false);
                pool.PooledObjectsQueue.Enqueue(go);
                pool.ActiveList.RemoveAt(0);
            }
        }
    }

   
    public GameObject Spawn(string poolTag, Vector3 position)
    {
        var obj = SpawnFromPool(poolTag);

        obj.transform.position = position;
        return obj;
    }

 
    public GameObject Spawn(string poolTag, Vector3 position, Quaternion rotation, Transform parent, bool worldPosition = true)
    {
        var obj = SpawnFromPool(poolTag);

        if (worldPosition)
            obj.transform.position = position;
        else
            obj.transform.localPosition = position;
        obj.transform.rotation = rotation;
        obj.transform.SetParent(parent);
        return obj;
    }

    private GameObject SpawnFromPool(string poolTag)
    {
        if (!poolDictionary.TryGetValue(poolTag, out var pooled)) return null;
        if (!pooled.CountInactive.Equals(0))
        {
            var obj = poolDictionary[poolTag].PooledObjectsQueue.Dequeue();
            obj.transform.localScale = Vector3.one;
            obj.SetActive(true);
            pooled.ActiveList.Add(obj);
            // If pool cannot grow more, 
            if (pooled.CountAll.Equals(pooled.CountMax))
            {
                poolDictionary[poolTag].PooledObjectsQueue.Enqueue(obj);
            }

            return obj;
        }
        else if (!pooled.CountAll.Equals(pooled.CountMax))
        {
            var obj = Instantiate(pooled.ActiveList[0], transform);
            obj.transform.localScale = Vector3.one;
            pooled.ActiveList.Add(obj);
            pooled.CountAll++;
            obj.SetActive(true);

            return obj;
        }
        else
        {
            for (int i = 0; i < pooled.CountActive; i++)
                pooled.PooledObjectsQueue.Enqueue(pooled.ActiveList[i]);
            var obj = pooled.ActiveList[0];
            return obj;
        }
    }

    public void Release(GameObject pooledGameObject, string poolTag)
    {
        if (!poolDictionary.TryGetValue(poolTag, out var pooled)) return;
        if (!pooled.ActiveList.Contains(pooledGameObject)) return;
        pooledGameObject.SetActive(false);
        pooledGameObject.transform.SetParent(transform);
        if (pooled.PooledObjectsQueue.Contains(pooledGameObject)) return;

        pooled.PooledObjectsQueue.Enqueue(pooledGameObject);
        pooled.ActiveList.Remove(pooledGameObject);
    }

    public IEnumerable<GameObject> Peek(string poolTag)
    {
        return poolDictionary[poolTag].PooledObjectsQueue;
    }

  
    public void AddToPool(string poolTag, GameObject prefab, int count, int maxCount)
    {
        if (poolDictionary.ContainsKey(poolTag))
        {
            return;
        }

        if (count > maxCount)
        {
            return;
        }

        var queue = new Queue<GameObject>();
        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            queue.Enqueue(obj);
        }

        var pooled = new Pooled { PooledObjectsQueue = queue, CountAll = count, CountMax = maxCount };
        poolDictionary.Add(poolTag, pooled);
    }
}
