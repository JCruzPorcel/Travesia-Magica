using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyPooler : MonoBehaviour
{
    private Dictionary<string, float> enemyTagMap = new Dictionary<string, float>()
    {
        { "Folder", 3f },
        { "File", 4f },
    };

    private Dictionary<string, float> enemySpeedMap = new Dictionary<string, float>()
    {
        { "Folder", 30f },
        { "File", 50f },
    };

    string folderTag;
    string fileTag;

    float folderTimer;
    float fileTimer;

    float lastPos = 0f;

    private Dictionary<string, List<GameObject>> poolListMap = new Dictionary<string, List<GameObject>>();
    private Dictionary<string, Queue<GameObject>> poolQueueMap = new Dictionary<string, Queue<GameObject>>();

    Transform playerTransform;

    private void Start()
    {
        folderTag = enemyTagMap.ElementAt(0).Key;
        fileTag = enemyTagMap.ElementAt(1).Key;

        playerTransform = GameObject.FindWithTag("Player").transform;

        foreach (string enemyType in enemyTagMap.Keys)
        {
            poolListMap[enemyType] = ObjectPooler.Instance.GetPool(ObjectType.Enemy, enemyType);

        }

        folderTimer = enemyTagMap[folderTag];
        fileTimer = enemyTagMap[fileTag];
    }

    private void Update()
    {
        if (GameManager.Instance.currentGameState == GameState.InGame && GameManager.Instance.currentGameFlowState == GameFlowState.Normal)
        {
            AddToQueue();
            Timer();
            SpawnFromQueue(folderTag, ref folderTimer);
            SpawnFromQueue(fileTag, ref fileTimer);
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

    private void SpawnFromQueue(string tag, ref float timer)
    {
        if (enemyTagMap.ContainsKey(tag))
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
                    go.GetComponent<Enemy>().speed = enemySpeedMap[tag];
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

        /*float spawnInPlayerPos = Random.Range(0f, 2f);

        if (spawnInPlayerPos < .1)
        {
            direction.y = playerTransform.position.y - 5;
            if (direction.y <= -17)
            {
                direction.y = playerTransform.position.y + 5;
            }
        }*/

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

    public void UpdateValue(string key, float value)
    {
        if (enemyTagMap.ContainsKey(key))
        {
            enemyTagMap[key] = value;


            if (key == "File")
            {
                fileTimer = value;
            }
            else if (key == "Folder")
            {
                folderTimer = value;
            }
        }
        else
        {
            Debug.LogWarning($"Not found key: {key}");
        }
    }

    public void UpdateSpeed(string key, float speed)
    {
        if (enemyTagMap.ContainsKey(key))
        {
            foreach (GameObject enemy in poolListMap[key])
            {
                enemy.GetComponent<Enemy>().speed = speed;
            }
            enemySpeedMap[key] = speed;
        }
    }
}
