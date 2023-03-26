using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public List<GameObject> poolList = new List<GameObject>();
    private Queue<GameObject> objectQueue = new Queue<GameObject>();

    [Min(0.25f)] public float spawnFrequency = 1f;
    private float timer;

    private int lastYPosition;
    private readonly int xPosition = 50;
    private readonly int minYPosition = -21;
    private readonly int maxYPosition = 21;
    private readonly int yPositionRange = 5;

    private void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            SpawnObject();
        }

        CheckObjectStatus();
    }

    private void CheckObjectStatus()
    {
        foreach (GameObject obj in poolList)
        {
            if (!obj.activeInHierarchy)
            {
                objectQueue.Enqueue(obj);
            }
        }
    }

    private void SpawnObject()
    {
        if (objectQueue.Count > 0)
        {
            GameObject go = objectQueue.Peek();

            if (go.activeInHierarchy)
            {
                return;
            }

            int yPos = Random.Range(minYPosition, maxYPosition);

            while (Mathf.Abs(yPos - lastYPosition) <= yPositionRange)
            {
                yPos = Random.Range(minYPosition, maxYPosition);
            }

            lastYPosition = yPos;

            go.transform.position = new Vector2(xPosition, yPos);
            go.SetActive(true);
            objectQueue.Dequeue();


            timer = Random.Range(0.25f, spawnFrequency);
        }
    }
}
