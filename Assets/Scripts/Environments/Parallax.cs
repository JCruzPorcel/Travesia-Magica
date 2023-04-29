using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public List<GameObject> cloudPool = new List<GameObject>();
    Queue<GameObject> cloudQueue = new Queue<GameObject>();

    const string cloudTag = "Cloud";
    [SerializeField] float spawnInterval = 1.5f;
    const float minSpawnDistance = 10.0f;
    const float minX = 50.0f;
    const float minY = -20.0f;
    const float maxY = 20.0f;
    const float minSize = 0.7f;
    const float maxSize = 2.0f;
    const float minSpeed = 10.0f;
    const float maxSpeed = 16.0f;

    private float spawnTimer = 0.0f;

    private void Start()
    {
        cloudPool = ObjectPooler.Instance.GetPool(ObjectType.Environment, cloudTag);
        ShuffleList(cloudPool);
        SpawnCloud();
    }

    private void Update()
    {
        if (GameManager.Instance.currentGameState == GameState.InGame)
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnInterval)
            {
                SpawnCloud();
            }
        }
    }

    private void SpawnCloud()
    {
        int spawnCount = Random.Range(1, 4);
        float lastSpawnedY = minY - minSpawnDistance;
        float lastYPos = lastSpawnedY;

        foreach (GameObject cloud in cloudPool)
        {
            if (!cloud.activeInHierarchy && !cloudQueue.Contains(cloud))
            {
                cloudQueue.Enqueue(cloud);
            }

        }

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject cloud = cloudQueue.Count > 0 ? cloudQueue.Dequeue() : Instantiate(cloudPool[Random.Range(0, 2)]);

            float newYPos = Random.Range(lastSpawnedY + minSpawnDistance, maxY);
            while (Mathf.Abs(newYPos - lastYPos) < 5f)
            {
                newYPos = Random.Range(lastSpawnedY + minSpawnDistance, maxY);
            }

            Vector3 spawnPosition = new Vector3(
                minX,
                newYPos,
                0.0f
            );

            cloud.transform.position = spawnPosition;
            cloud.transform.localScale = Vector3.one * Random.Range(minSize, maxSize);

            CloudController cloudController = cloud.GetComponent<CloudController>();
            cloudController.Speed = Random.Range(minSpeed, maxSpeed);

            lastSpawnedY = spawnPosition.y;
            lastYPos = newYPos;

            cloud.SetActive(true);
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
