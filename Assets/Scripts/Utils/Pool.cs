using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    public List<GameObject> poolList = new List<GameObject>();
    Queue<GameObject> objectQueue = new Queue<GameObject>();

    [Min(.25f)] public float spawnFrecuency = 1f;

    float _timer;
    int xPos = 50;

    int lastPos;

    private void Update()
    {
        if (_timer > 0f)
        {
            _timer -= Time.deltaTime;
        }
        else
        {
            SpawnObject();
        }

        CheckObjectStatus();
    }


    void CheckObjectStatus()
    {
        foreach (GameObject obj in poolList)
        {
            if (!obj.activeInHierarchy)
            {
                objectQueue.Enqueue(obj);
            }
        }
    }


    void SpawnObject()
    {
        if (objectQueue.Count > 0)
        {
            GameObject go = objectQueue.Peek();

            if (go.activeInHierarchy)
            {
                return;
            }

            int yPos = Random.Range(-21, 22);

            while (Mathf.Abs(yPos - lastPos) <= 5)
            {
                yPos = Random.Range(-21, 22);
            }
            lastPos = yPos;

            go.transform.position = new Vector2(xPos, yPos);

            go.SetActive(true);

            objectQueue.Dequeue();

            _timer = Random.Range(.25f, spawnFrecuency);
        }
    }

}
