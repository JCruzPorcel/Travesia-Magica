using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemPool : MonoBehaviour
{
    private readonly Dictionary<string, float> itemTagMap = new Dictionary<string, float>()
    {
        { "Heal", 20f },
    };

    string healTag;

    float healTimer;

    float lastPos = 0f;

    private readonly Dictionary<string, List<GameObject>> poolListMap = new Dictionary<string, List<GameObject>>();
    private readonly Dictionary<string, Queue<GameObject>> poolQueueMap = new Dictionary<string, Queue<GameObject>>();


    private void Start()
    {
        healTag = itemTagMap.ElementAt(0).Key;

        foreach (string itemType in itemTagMap.Keys)
        {
            poolListMap[itemType] = ObjectPooler.Instance.GetPool(ObjectType.Item, itemType);
        }
    }

    private void Update()
    {
        if (GameManager.Instance.currentGameState == GameState.InGame)
        {
            AddToQueue();
            Timer();
            SpawnFromQueue(ref healTimer, healTag);
        }
    }

    private void AddToQueue()
    {
        foreach (string itemType in itemTagMap.Keys)
        {
            List<GameObject> poolList = poolListMap[itemType];
            Queue<GameObject> poolQueue;
            if (!poolQueueMap.TryGetValue(itemType, out poolQueue))
            {
                poolQueue = new Queue<GameObject>();
                poolQueueMap[itemType] = poolQueue;
            }

            foreach (GameObject item in poolList)
            {
                if (!item.activeInHierarchy && !poolQueueMap[itemType].Contains(item))
                {
                    poolQueueMap[itemType].Enqueue(item);
                }
            }
        }
    }

    private void SpawnFromQueue(ref float timer, string tag)
    {
        if (timer <= 0f)
        {
            timer = itemTagMap[tag];

            if (poolQueueMap[tag].Count > 0)
            {
                GameObject go = poolQueueMap[tag].Peek();
                go.transform.position = GetNewPosition();
                go.SetActive(true);
                poolQueueMap[tag].Dequeue();
            }
            else
            {
                GameObject poolParent = GameObject.Find($"Item {tag} Pool");

                if (poolParent == null)
                {
                    poolParent = new GameObject($"Item {tag} Pool");
                }

                GameObject prefab = ObjectPooler.Instance.GetPrefab(ObjectType.Item, tag);
                GameObject go = Instantiate(prefab, poolParent.transform);
                go.transform.position = GetNewPosition();
                go.name = tag;
                poolListMap[tag].Add(go);
            }

        }
    }

    private Vector3 GetNewPosition()
    {
        Vector3 direction = new Vector3(50f, Random.Range(-22, 22), 0f);

        while (Mathf.Abs(lastPos - direction.y) < 5)
        {
            direction.y = Random.Range(-22, 22);
        }

        lastPos = direction.y;
        return direction;
    }

    private void Timer()
    {
        if (healTimer > 0f)
        {
            healTimer -= Time.deltaTime;
        }
    }
}
