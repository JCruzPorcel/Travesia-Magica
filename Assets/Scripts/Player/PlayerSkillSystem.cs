using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillSystem : MonoBehaviour
{
    [SerializeField] GameObject shieldPrefab;
    [SerializeField] float shieldCooldown;
    [SerializeField] float shieldDuration;
    [SerializeField] float rotationSpeed;
    private float shieldTimer;
    private bool shieldEnabled;
    private PlayerControls playerInputs;

    const string invulnerableTag = "Invulnerable";
    const string playerTag = "Player";

    private void Awake()
    {
        playerInputs = new PlayerControls();
        playerInputs.Player.Enable();
        playerInputs.Player.Shield.performed += EnableShield;
    }

    private void Update()
    {
        if (shieldEnabled)
        {
            // Rotar el escudo en el eje Z
            shieldPrefab.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

            if (shieldTimer > 0f)
            {
                shieldTimer -= Time.deltaTime;

                if (shieldTimer <= 0f)
                {
                    DisableShield();
                }
            }
        }
        else if (shieldTimer > 0f)
        {
            shieldTimer -= Time.deltaTime;
        }
    }

    private void EnableShield(InputAction.CallbackContext context)
    {
        if (!shieldEnabled && shieldTimer <= 0f)
        {
            shieldEnabled = true;
            shieldPrefab.SetActive(true);
            shieldTimer = shieldDuration;
            gameObject.tag = invulnerableTag;
        }
    }

    private void DisableShield()
    {
        shieldPrefab.SetActive(false);
        shieldEnabled = false;
        shieldTimer = shieldCooldown;
        gameObject.tag = playerTag;

    }

    private void OnDestroy()
    {
        playerInputs.Player.Shield.performed -= EnableShield;
    }
}
