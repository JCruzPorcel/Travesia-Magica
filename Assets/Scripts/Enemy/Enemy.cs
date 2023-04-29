using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Range(0f, 50f)] public float speed = 10f;
    [HideInInspector] public float currentHp;
    [Min(0f)] public int maxHp;
    [Range(0f, 25f)] public int damage;

    public float attackRange;
    public Transform attackPoint;

    public LayerMask playerLayer;
    public Animator animator;
    public ParticleSystem particles;

    GameManager gameManager;
    FloatingTextPool textPool;

    private void Start()
    {
        currentHp = maxHp;

        gameManager = GameManager.Instance;

        animator = GetComponentInChildren<Animator>();

        particles = GetComponentInChildren<ParticleSystem>();
        textPool = FindFirstObjectByType<FloatingTextPool>();
    }

    private void Update()
    {
        if (gameManager.currentGameState == GameState.InGame)
        {
            DespawnDistance();
            Attack();

            animator.speed = 1f;

            if (particles != null)
                particles.Play();
        }
        else
        {
            animator.speed = 0f;

            if (particles != null)
                particles.Pause();
        }
    }

    private void FixedUpdate()
    {
        if (gameManager.currentGameState == GameState.InGame)
        {
            Movement();
        }
    }

    public void PopUpDamage(float damage)
    {
        GameObject go = textPool.GetQueue();

        go.GetComponentInChildren<TextMesh>().text = damage.ToString();

        go.transform.position = transform.position;

        go.SetActive(true);
    }

    private void DespawnDistance()
    {
        if (transform.position.x <= -50f)
        {
            this.gameObject.SetActive(false);
        }
    }

    public virtual void TakenDamage(float damage)
    {
        currentHp -= damage;

        PopUpDamage(damage);

        if (currentHp <= 0f)
        {
            this.gameObject.SetActive(false);
        }
    }

    public virtual void Movement()
    {
        transform.position += Vector3.left * speed * Time.fixedDeltaTime;
    }

    public virtual void ApplyKnockback(Vector2 direction, float force)
    {
        transform.Translate(direction.normalized * force * Time.deltaTime);
    }

    public virtual void Attack()
    {
        Vector2 size = new Vector2(attackRange, attackRange);

        Collider2D[] hitPlayer = Physics2D.OverlapBoxAll(attackPoint.position, size, playerLayer);

        foreach (Collider2D player in hitPlayer)
        {
            player.GetComponent<HealthSystem>().Damage(damage);
            this.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        currentHp = maxHp;
    }
}
