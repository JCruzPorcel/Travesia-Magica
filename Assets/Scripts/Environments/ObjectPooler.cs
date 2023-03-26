using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolableObject
{
    public string name;
    public int amount;
    public float timeToSpawn;
    public GameObject prefab;
}

public class ObjectPooler : MonoBehaviour
{
    public List<PoolableObject> poolableObjects = new List<PoolableObject>();
    public List<PoolableBullets> poolableBullets = new List<PoolableBullets>();

    private void Start()
    {
        ObjectSpawner();
        BulletSpawner();
    }

    private void ObjectSpawner()
    {
        foreach (PoolableObject poolableObject in poolableObjects)
        {
            GameObject poolParent = new GameObject(poolableObject.name + " Pool");
            poolParent.AddComponent<ObjectPool>();

            ObjectPool objectPool = poolParent.GetComponent<ObjectPool>();
            objectPool.spawnFrequency = poolableObject.timeToSpawn;

            for (int i = 0; i < poolableObject.amount; i++)
            {
                GameObject pooledObject = Instantiate(poolableObject.prefab, poolParent.transform);
                pooledObject.SetActive(false);

                objectPool.poolList.Add(pooledObject);
            }
        }
    }

    [System.Serializable]
    public class PoolableBullets
    {
        public string name;
        public int amount;
        public GameObject prefab;
    }
    private void BulletSpawner()
    {
        foreach (PoolableBullets poolableBullets in poolableBullets)
        {
            GameObject poolParent = new GameObject(poolableBullets.name + " Pool");

            for (int i = 0; i < poolableBullets.amount; i++)
            {
                GameObject pooledObject = Instantiate(poolableBullets.prefab, poolParent.transform);
                pooledObject.SetActive(false);
            }
        }
    }
}

