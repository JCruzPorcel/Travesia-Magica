using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCombat : MonoBehaviour
{
    private PlayerControls playerInput;

    private bool isBasicShootButtonPressed;
    //private bool isSecondaryShootButtonPressed;
    private bool isSpecialShootButtonPressed;

    private Queue<GameObject> basicBulletQueue = new Queue<GameObject>();
    //private Queue<GameObject> secondaryBulletQueue = new Queue<GameObject>();
    private Queue<GameObject> specialBulletQueue = new Queue<GameObject>();

    private List<GameObject> basicBulletPool;
    //private List<GameObject> secondaryBulletPool;
    private List<GameObject> specialBulletPool;

    private float timeSinceLastBasicShot;
    //private float timeSinceLastSecondaryShot;
    private float timeSinceLastSpecialShot;

    [SerializeField] private float basicShot_FireRate = 0.1f;
   // [SerializeField] private float secondaryShot_FireRate = 0.2f;
    [SerializeField] private float specialShot_FireRate = 0.5f;

    [SerializeField] private Transform shotPoint;

    private const string basicBulletTag = "Basic Bullet";
  //  private const string secondaryBulletTag = "Secondary Bullet";
    private const string specialBulletTag = "Special Bullet";

    private void Awake()
    {
        playerInput = new PlayerControls();
        playerInput.Player.Enable();

        playerInput.Player.BasicShoot.started += context => isBasicShootButtonPressed = true;
        playerInput.Player.BasicShoot.canceled += context => isBasicShootButtonPressed = false;

       // playerInput.Player.SecondaryShoot.started += context => isSecondaryShootButtonPressed = true;
       // playerInput.Player.SecondaryShoot.canceled += context => isSecondaryShootButtonPressed = false;

        playerInput.Player.SpecialShoot.started += context => isSpecialShootButtonPressed = true;
        playerInput.Player.SpecialShoot.canceled += context => isSpecialShootButtonPressed = false;
    }

    private void Start()
    {
        basicBulletPool = ObjectPooler.Instance.GetPool(ObjectType.Player, basicBulletTag);
      //  secondaryBulletPool = ObjectPooler.Instance.GetPool(ObjectType.Player, secondaryBulletTag);
        specialBulletPool = ObjectPooler.Instance.GetPool(ObjectType.Player, specialBulletTag);
    }

    private void Update()
    {
        if (GameManager.Instance.currentGameState == GameState.InGame)
        {
            AddObjectsToQueue(basicBulletPool, basicBulletQueue);
            //AddObjectsToQueue(secondaryBulletPool, secondaryBulletQueue);
            AddObjectsToQueue(specialBulletPool, specialBulletQueue);

            if (isBasicShootButtonPressed)
            {
                ShootRepeatedly(basicBulletQueue, basicBulletPool, ref timeSinceLastBasicShot, basicShot_FireRate, basicBulletTag);
            }
           /* else if (isSecondaryShootButtonPressed)
            {
                ShootRepeatedly(secondaryBulletQueue, secondaryBulletPool, ref timeSinceLastSecondaryShot, secondaryShot_FireRate, secondaryBulletTag);
            }*/
            else if (isSpecialShootButtonPressed)
            {
                ShootRepeatedly(specialBulletQueue, specialBulletPool, ref timeSinceLastSpecialShot, specialShot_FireRate, specialBulletTag);
            }

        }
    }

    public void AddObjectsToQueue(List<GameObject> list, Queue<GameObject> queue)
    {
        foreach (GameObject obj in list)
        {
            if (!obj.activeInHierarchy && !queue.Contains(obj))
            {
                queue.Enqueue(obj);
            }
        }
    }

    private void ShootRepeatedly(Queue<GameObject> queue, List<GameObject> list, ref float timeSinceLastShot, float fireRate, string tag)
    {
        if (Time.time - timeSinceLastShot < fireRate)
        {
            return;
        }

        Shoot(list, queue, tag);

        timeSinceLastShot = Time.time;
    }

    public void Shoot(List<GameObject> list, Queue<GameObject> queue, string tag)
    {
        if (queue.Count > 0)
        {
            GameObject bullet = queue.Dequeue();
            bullet.transform.position = shotPoint.position;
            bullet.SetActive(true);
        }
        else
        {
            GameObject poolParent = GameObject.Find($"Player {tag} Pool");

            if (poolParent == null)
            {
                poolParent = new GameObject($"Player {tag} Pool");
            }

            GameObject prefab = ObjectPooler.Instance.GetPrefab(ObjectType.Player, tag);
            GameObject go = Instantiate(prefab, poolParent.transform);
            go.transform.position = shotPoint.position;
            go.name = tag;
            list.Add(go);
        }
    }

}