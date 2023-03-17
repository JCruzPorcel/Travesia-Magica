using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public class Object
{
    public string name;
    public int amount;
    public float timeToSpawn;
    public GameObject item;
}

public class ObjectPooler : MonoBehaviour
{
    public List<Object> objectPool = new List<Object>();


    private void Start()
    {
        foreach (Object obj in objectPool)
        {
            GameObject parentObject = new GameObject(obj.name + " Pool");

            parentObject.AddComponent<Pool>();

            Pool parentPool = parentObject.GetComponent<Pool>();

            for (int i = 0; i < obj.amount; i++)
            {
                GameObject go = Instantiate(obj.item, parentObject.transform);

                go.SetActive(false);

                parentPool.poolList.Add(go);
                parentPool.spawnFrecuency = obj.timeToSpawn;
            }
        }
    }
}
