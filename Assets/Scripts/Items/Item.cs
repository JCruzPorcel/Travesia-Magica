using UnityEngine;

public class Item : MonoBehaviour
{
    [Range(0f, 100f)]public float speed = 1f;

    public float attackRange;
    public Transform attackPoint;

    public LayerMask playerLayer;
    GameManager gameManager;
    [HideInInspector] public Animator animator;
    public Vector2 size;

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
            ItemPick();
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

    public virtual void ItemPick()
    {
        size = new Vector2(attackRange, attackRange);

        Collider2D[] hitPlayer = Physics2D.OverlapBoxAll(attackPoint.position, size, playerLayer);

        foreach (Collider2D player in hitPlayer)
        {
            if (player.CompareTag("Player") || player.CompareTag("Invulnerable"))
            {
                ObjectAtribute();
                this.gameObject.SetActive(false);
            }
        }
    }
}
