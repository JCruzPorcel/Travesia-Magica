using UnityEngine;

[DisallowMultipleComponent]
public class PetController : MonoBehaviour
{
    public Transform player;
    public float maxRange;
    public float minRange;
    public float moveSpeed;
    private GameObject petGo;
    Transform petSpawnPoint;

    Animator anim;
    SpriteRenderer sr;
    ParticleSystem particles;

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;

        player = GameObject.FindWithTag("Player").transform;

        petSpawnPoint = this.transform;
        PetSpawner();

        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        particles = GetComponentInChildren<ParticleSystem>();

        anim.speed = 1;

        if (particles != null)
            particles.Play();
    }

    void FixedUpdate()
    {
        if (gameManager.currentGameState == GameState.InGame)
        {
            FollowPlayer();
            anim.speed = 1;

            if (particles != null)
                particles.Play();
        }
        else
        {
            anim.speed = 0;

            if (particles != null)
                particles.Pause();
        }
    }

    private void FollowPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > maxRange)
        {
            Vector2 movement = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            transform.position = movement;

        }
        else if (distanceToPlayer < minRange)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, -moveSpeed * Time.deltaTime);
        }

        if (transform.position.x < player.position.x + .05f)
        {
            sr.flipX = true;
        }
        else if (transform.position.x > player.position.x - .05f)
        {
            sr.flipX = false;
        }
    }

    private void PetSpawner()
    {
        petGo = gameManager.PetSelected.Prefab;

        Instantiate(petGo, petSpawnPoint);
    }
}