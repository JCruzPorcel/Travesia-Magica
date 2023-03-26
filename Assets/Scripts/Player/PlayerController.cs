using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Range(0f, 50f)] private float speed = 25f;
    private Vector2 direction;
    private GameManager gameManager;
    private Animator animator;

    private GameObject characterGo;
    [SerializeField] Transform playerSpawnPoint;


    private void Awake()
    {
        PlayerInputs playerInputs = new PlayerInputs();
        playerInputs.Player.Enable();

        playerInputs.Player.Movement.performed += OnMove;
        playerInputs.Player.Movement.canceled += OnMove;
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        PlayerSpawner();        
        animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        if (gameManager.currentGameState == GameState.InGame)
        {
            Movement();

            animator.speed = 1f;
        }
        else
        {
            animator.speed = 0f;
        }
    }

    private void Movement()
    {
        transform.position += (Vector3)direction * speed * Time.fixedDeltaTime;
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
}