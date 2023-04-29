using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyPooler : MonoBehaviour
{
    private readonly Dictionary<string, float> enemyTagMap = new Dictionary<string, float>()
    {
        { "Folder", 3f },
        { "File", 4f },
    };

    string folderTag;
    string fileTag;

    float folderTimer;
    float fileTimer;

    float lastPos = 0f;

    private readonly Dictionary<string, List<GameObject>> poolListMap = new Dictionary<string, List<GameObject>>();
    private readonly Dictionary<string, Queue<GameObject>> poolQueueMap = new Dictionary<string, Queue<GameObject>>();


    private void Start()
    {
        folderTag = enemyTagMap.ElementAt(0).Key;
        fileTag = enemyTagMap.ElementAt(1).Key;


        foreach (string enemyType in enemyTagMap.Keys)
        {
            poolListMap[enemyType] = ObjectPooler.Instance.GetPool(ObjectType.Enemy, enemyType);
        }
    }

    private void Update()
    {
        if (GameManager.Instance.currentGameState == GameState.InGame)
        {
            AddToQueue();
            Timer();
            SpawnFromQueue(ref folderTimer, folderTag);
            SpawnFromQueue(ref fileTimer, fileTag);
        }
    }

    private void AddToQueue()
    {
        foreach (string enemyType in enemyTagMap.Keys)
        {
            List<GameObject> poolList = poolListMap[enemyType];
            Queue<GameObject> poolQueue;
            if (!poolQueueMap.TryGetValue(enemyType, out poolQueue))
            {
                poolQueue = new Queue<GameObject>();
                poolQueueMap[enemyType] = poolQueue;
            }

            foreach (GameObject enemy in poolList)
            {
                if (!enemy.activeInHierarchy && !poolQueueMap[enemyType].Contains(enemy))
                {
                    poolQueueMap[enemyType].Enqueue(enemy);
                }
            }
        }
    }

    private void SpawnFromQueue(ref float timer, string tag)
    {
        if (timer <= 0f)
        {
            timer = enemyTagMap[tag];

            if (poolQueueMap[tag].Count > 0)
            {
                GameObject go = poolQueueMap[tag].Peek();
                go.transform.position = GetNewPosition();
                go.SetActive(true);
                poolQueueMap[tag].Dequeue();
            }
            else
            {
                GameObject poolParent = GameObject.Find($"Enemy {tag} Pool");

                if (poolParent == null)
                {
                    poolParent = new GameObject($"Enemy {tag} Pool");
                }

                GameObject prefab = ObjectPooler.Instance.GetPrefab(ObjectType.Enemy, tag);
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
        if (folderTimer > 0f)
        {
            folderTimer -= Time.deltaTime;
        }

        if (fileTimer > 0f)
        {
            fileTimer -= Time.deltaTime;
        }
    }
}
