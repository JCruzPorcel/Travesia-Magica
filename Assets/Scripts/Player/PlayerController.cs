using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    [Range(0f, 50f)][SerializeField] private float speed = 25f;
    private Vector2 direction;
    public  Sprite[] sprites;

    private void Awake()
    {
        PlayerInputs playerInputs = new PlayerInputs();
        playerInputs.Player.Enable();

        playerInputs.Player.Movement.performed += OnMove;
        playerInputs.Player.Movement.canceled += OnMove;
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        transform.position += (Vector3)direction * speed * Time.fixedDeltaTime;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
    }
}