using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    Enemy,
    Item,
    Player
}

[System.Serializable]
public class Pool
{
    public string tag;
    public GameObject prefab;
    public int size;
    public ObjectType type;
}

public class ObjectPooler : Singleton<ObjectPooler>
{
    private Dictionary<ObjectType, Dictionary<string, Stack<GameObject>>> poolDictionary = new Dictionary<ObjectType, Dictionary<string, Stack<GameObject>>>();

    private Dictionary<ObjectType, Dictionary<string, GameObject>> objDictionary= new Dictionary<ObjectType, Dictionary<string, GameObject>>();

    [SerializeField] private List<Pool> pools = new List<Pool>();

    private void Start()
    {
        CreatePools();
    }

    private void CreatePools()
    {
        foreach (Pool pool in pools)
        {
            GameObject poolParent = GameObject.Find($"{pool.type} {pool.tag} Pool");

            if (poolParent == null)
            {
                poolParent = new GameObject($"{pool.type} {pool.tag} Pool");
            }

            for (int i = 0; i < pool.size; i++)
            {
                GameObject go = Instantiate(pool.prefab, poolParent.transform);
                go.name = $"{pool.tag} {i}";

                if (!poolDictionary.ContainsKey(pool.type))
                {
                    poolDictionary[pool.type] = new Dictionary<string, Stack<GameObject>>();
                    objDictionary[pool.type] = new Dictionary<string, GameObject>();
                }

                if (!poolDictionary[pool.type].ContainsKey(pool.tag))
                {
                    poolDictionary[pool.type][pool.tag] = new Stack<GameObject>();
                    objDictionary[pool.type][pool.tag] = pool.prefab;
                }


                if (!objDictionary.ContainsKey(pool.type))
                {
                    objDictionary[pool.type] = new Dictionary<string, GameObject>();
                }

                if (!objDictionary[pool.type].ContainsKey(pool.tag))
                {
                    objDictionary[pool.type][pool.tag] = pool.prefab;
                }

                poolDictionary[pool.type][pool.tag].Push(go);
                go.SetActive(false);
            }
        }
    }

    public List<GameObject> GetPool(ObjectType type, string tag)
    {
        List<GameObject> list = new List<GameObject>();

        if (poolDictionary.ContainsKey(type) && poolDictionary[type].ContainsKey(tag))
        {
            Stack<GameObject> stack = poolDictionary[type][tag];

            while (stack.Count > 0)
            {
                GameObject go = stack.Pop();
                list.Add(go);
            }
        }
        else
        {
            Debug.LogWarning($"Pool with type {type} and tag {tag} not found.");
        }

        return list;
    }

    public GameObject GetPrefab(ObjectType type, string tag)
    {
        GameObject go = null;

        if (objDictionary.ContainsKey(type) && objDictionary[type].ContainsKey(tag))
        {
            go = objDictionary[type][tag];
        }
        else
        {
            Debug.LogWarning($"Pool with type {type} and tag {tag} not found.");
        }
        return go;
    }
}
