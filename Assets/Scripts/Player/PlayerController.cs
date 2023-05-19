using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Range(0f, 50f)] private float speed = 25f;
    [SerializeField, Range(0f, 5f)] private float cameraMargin = 1f;

    private Vector2 direction;
    private GameManager gameManager;
    private Animator animator;
    private ParticleSystem particles;
    private SpriteRenderer playerSpriteRenderer;
    private GameObject characterGo;
    Transform playerSpawnPoint;

    [SerializeField] private float xMin, xMax, yMin, yMax; // límites de la cámara

    private void Awake()
    {
        PlayerControls playerInputs = new PlayerControls();
        playerInputs.Player.Enable();

        playerInputs.Player.Movement.performed += OnMove;
        playerInputs.Player.Movement.canceled += OnMove;
        particles = GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        playerSpawnPoint = this.transform;
        PlayerSpawner();

        animator = GetComponentInChildren<Animator>();
        playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();

        animator.speed = 1f;

        if (particles != null)
            particles.Play();

        // Calcula los límites de la cámara
        //CameraBounds();       
    }


    private void FixedUpdate()
    {
        if (gameManager.currentGameState == GameState.InGame)
        {
            Movement();

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

    private void Movement()
    {
        // Limita el movimiento del jugador dentro de los límites de la cámara
        Vector3 newPosition = transform.position + (Vector3)direction * speed * Time.fixedDeltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, xMin, xMax);
        newPosition.y = Mathf.Clamp(newPosition.y, yMin, yMax);
        transform.position = newPosition;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
    }

    private void PlayerSpawner()
    {
        characterGo = gameManager.CharacterSelected.Prefab;

        Instantiate(characterGo, playerSpawnPoint);
    }

    void CameraBounds()
    {
        Camera gameCamera = Camera.main;
        float cameraHeight = 2f * gameCamera.orthographicSize;
        float cameraWidth = cameraHeight * gameCamera.aspect;
        xMin = gameCamera.transform.position.x - cameraWidth / 2f + playerSpriteRenderer.bounds.size.x / 2f + cameraMargin;
        xMax = gameCamera.transform.position.x + cameraWidth / 2f - playerSpriteRenderer.bounds.size.x / 2f - cameraMargin;
        yMin = gameCamera.transform.position.y - cameraHeight / 2f + playerSpriteRenderer.bounds.size.y / 2f + cameraMargin;
        yMax = gameCamera.transform.position.y + cameraHeight / 2f - playerSpriteRenderer.bounds.size.y / 2f - cameraMargin;
    }
}
