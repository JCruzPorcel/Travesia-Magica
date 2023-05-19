using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public List<GameObject> cloudPool = new List<GameObject>();
    Queue<GameObject> cloudQueue = new Queue<GameObject>();

    const string cloudTag = "Cloud";
    const float minSpawnDistance = 5.0f;
    const float minX = 60.0f;
    const float maxX = 70.0f;
    const float minY = -22.0f;
    const float maxY = 22.0f;
    const float minSize = 0.7f;
    const float maxSize = 2.0f;
    const float minSpeed = 10.0f;
    const float maxSpeed = 22.0f;

    [SerializeField] float spawnInterval = 8f;
    private float spawnTimer = 0.0f;

    private float lastYPos;

    private void Start()
    {
        cloudPool = ObjectPooler.Instance.GetPool(ObjectType.Environment, cloudTag);
        ShuffleList(cloudPool);
        EnqueueCloud();
        SpawnCloud();
    }

    private void Update()
    {
        if (GameManager.Instance.currentGameState == GameState.InGame)
        {
            EnqueueCloud();

            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnInterval)
            {
                SpawnCloud();
            }
        }
    }

    private void EnqueueCloud()
    {
        foreach (GameObject cloud in cloudPool)
        {
            if (!cloud.activeInHierarchy && !cloudQueue.Contains(cloud))
            {
                cloudQueue.Enqueue(cloud);
            }

        }
    }

    private void SpawnCloud()
    {
        int spawnCount = Random.Range(1, 4);
        float newYPos = Random.Range(minY, maxY);
        bool isNewCloud = true; 

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject cloud = cloudQueue.Count > 0 ? cloudQueue.Dequeue() : Instantiate(cloudPool[Random.Range(0, 2)]);

            foreach (GameObject cloudInPool in cloudPool)
            {
                if (cloudInPool == cloud)
                {
                    isNewCloud = false;
                    break;
                }
            }


            if (isNewCloud)
            {
                GameObject poolParent = GameObject.Find($"Environment {cloudTag} Pool");

                if (poolParent == null)
                {
                    poolParent = new GameObject($"Environment {cloudTag} Pool");
                }

                cloud.name = cloudTag;
                cloud.transform.SetParent(poolParent.transform);

                cloudPool.Add(cloud);
            }

            isNewCloud = true;

            while (Mathf.Abs(newYPos - lastYPos) < minSpawnDistance)
            {
                newYPos = Random.Range(minY, maxY);
            }
            float newXPos = Random.Range(minX, maxX);

            cloud.transform.position = new Vector3(newXPos, newYPos, 0f);

            cloud.transform.localScale = Vector3.one * Random.Range(minSize, maxSize);

            CloudController cloudController = cloud.GetComponent<CloudController>();
            cloudController.Speed = Random.Range(minSpeed, maxSpeed);

            cloud.SetActive(true);
            lastYPos = newYPos;
        }

        spawnTimer = 0.0f;
    }


    private void ShuffleList(List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            GameObject temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}