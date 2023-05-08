using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemPooler : MonoBehaviour
{
    private Dictionary<string, float> itemTagMap = new Dictionary<string, float>()
    {
        { "Heal", 15f },
        { "Coin", 11f}
    };

    private Dictionary<string, float> itemSpeedMap = new Dictionary<string, float>()
    {
        { "Heal", 35f },
        { "Coin", 50f}
    };

    string healTag;
    float healTimer;

    string coinTag;
    float coinTimer;

    float lastPos = 0f;

    private Dictionary<string, List<GameObject>> poolListMap = new Dictionary<string, List<GameObject>>();
    private Dictionary<string, Queue<GameObject>> poolQueueMap = new Dictionary<string, Queue<GameObject>>();


    private void Start()
    {
        healTag = itemTagMap.ElementAt(0).Key;
        coinTag = itemTagMap.ElementAt(1).Key;

        foreach (string itemType in itemTagMap.Keys)
        {
            poolListMap[itemType] = ObjectPooler.Instance.GetPool(ObjectType.Item, itemType);
        }

        healTimer = itemTagMap[healTag];
        coinTimer = itemTagMap[coinTag];
    }

    private void Update()
    {
        if (GameManager.Instance.currentGameState == GameState.InGame && GameManager.Instance.currentGameFlowState == GameFlowState.Normal)
        {
            AddToQueue();
            Timer();
            SpawnFromQueue(healTag, ref healTimer);
            SpawnFromQueue(coinTag, ref coinTimer);
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

    private void SpawnFromQueue(string tag, ref float timer)
    {
        if (itemTagMap.ContainsKey(tag))
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
                    go.GetComponent<Item>().speed = itemSpeedMap[tag];
                    poolListMap[tag].Add(go);
                }
            }
        }
    }

    private Vector3 GetNewPosition()
    {
        Vector3 direction = new Vector3(50f, Random.Range(-20, 15), 0f);

        while (Mathf.Abs(lastPos - direction.y) < 5)
        {
            direction.y = Random.Range(-20, 15);
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

        if (coinTimer > 0f)
        {
            coinTimer -= Time.deltaTime;
        }
    }

    public void UpdateValue(string key, float value)
    {
        if (itemTagMap.ContainsKey(key))
        {
            itemTagMap[key] = value;


            if (key == "Heal")
            {
                healTimer = value;
            }
            else if (key == "Coin")
            {
                coinTimer = value;
            }
        }
        else
        {
            Debug.LogWarning($"Not found key: {key}");
        }
    }

    public void UpdateSpeed(string key, float speed)
    {
        if (itemTagMap.ContainsKey(key))
        {
            foreach (GameObject item in poolListMap[key])
            {
                item.GetComponent<Item>().speed = speed;
            }
            itemSpeedMap[key] = speed;
        }
    }
}
