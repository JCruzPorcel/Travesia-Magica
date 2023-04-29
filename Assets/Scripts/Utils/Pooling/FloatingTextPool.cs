using System.Collections.Generic;
using UnityEngine;

public class FloatingTextPool : MonoBehaviour
{
    public List<GameObject> pool = new List<GameObject>();
    Queue<GameObject> queue = new Queue<GameObject>();
    const string floatingTextTag = "Floating Text";
    private GameObject prefab;

    private void Start()
    {
        pool = ObjectPooler.Instance.GetPool(ObjectType.Environment, floatingTextTag);
        prefab = ObjectPooler.Instance.GetPrefab(ObjectType.Environment, floatingTextTag);
    }

    private void Update()
    {
        if (GameManager.Instance.currentGameState == GameState.InGame)
        {
            if (pool.Count > 0)
            {
                foreach (GameObject go in pool)
                {
                    if (!go.activeInHierarchy && !queue.Contains(go))
                    {
                        queue.Enqueue(go);
                    }
                }
            }
        }
    }

    public GameObject GetQueue()
    {
        GameObject go;

        if (queue.Count > 0)
        {
            go = queue.Dequeue();
            return go;
        }
        else
        {
            GameObject poolParent = GameObject.Find($"Environment {floatingTextTag} Pool");

            if (poolParent == null)
            {
                poolParent = new GameObject($"Environment {floatingTextTag} Pool");
            }

            go = Instantiate(prefab, poolParent.transform);

            pool.Add(go);

            return go;
        }
    }
}
