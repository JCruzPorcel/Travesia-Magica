using UnityEngine;

public class ItemBase : MonoBehaviour
{
    [Range(0f, 50f)][SerializeField] protected float speed = 1f;
    [HideInInspector] public Animator animator;

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (gameManager.currentGameState == GameState.InGame)
        {
            animator.speed = 1f;
            DespawnDistance();
        }
        else
        {
            animator.speed = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (gameManager.currentGameState == GameState.InGame)
        {
            ObjectMovement();
        }
    }


    public virtual void ObjectMovement()
    {
        transform.position += Vector3.left * speed * Time.fixedDeltaTime;
    }

    private void DespawnDistance()
    {
        if (transform.position.x <= -50f)
        {
            this.gameObject.SetActive(false);
        }
    }

    public virtual void ObjectAtribute() { }
}
