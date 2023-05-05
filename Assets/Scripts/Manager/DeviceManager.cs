using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum DeviceType
{
    KeyboardAndMouse,
    Gamepad
}

public class DeviceManager : SingletonPersistent<DeviceManager>
{
    [SerializeField] private DeviceType currentDevice = DeviceType.KeyboardAndMouse;
    [Space(15)]
    [SerializeField] private string gamepadDisconnectedText;
    [SerializeField] private string gamepadDetectedText;
    [SerializeField] private string gamepadReconnectedText;
    [SerializeField] private GameObject gamepadAlertPrefab;
    private GameObject gamepadAlertInstance;
    [SerializeField] GameObject selectedButton;
    private PlayerInput playerInput;


    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.onControlsChanged += OnControlsChanged;
        InputSystem.onDeviceChange += OnDeviceChange;

        CheckSelectedButtonIsNull();

        // Verificar si hay un gamepad conectado al inicio
        if (Gamepad.current != null)
        {
            DeviceStatus($"Gamepad {gamepadDetectedText}");
            SetDevice(DeviceType.Gamepad);
        }
    }

    public void CheckSelectedButtonIsNull()
    {
        if (selectedButton == null)
        {
            GameManager gameManger = GetComponent<GameManager>();
            if (gameManger.currentGameState == GameState.MainMenu)
            {
                selectedButton = GameObject.Find("Play_Btn");
            }
        }
    }

    private void OnControlsChanged(PlayerInput input)
    {
        if (input.currentControlScheme == currentDevice.ToString())
            return;

        SetDevice(input.currentControlScheme == DeviceType.Gamepad.ToString() ? DeviceType.Gamepad : DeviceType.KeyboardAndMouse);
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if ((device is not Gamepad))
            return;

        switch (change)
        {
            case InputDeviceChange.Added:
                DeviceStatus($"Gamepad {gamepadDetectedText}");
                break;
            case InputDeviceChange.Disconnected:
                DeviceStatus($"Gamepad {gamepadDisconnectedText}");

                if (GameManager.Instance.currentGameState == GameState.InGame)
                {
                    GameManager.Instance.PauseGame();
                    OptionsManager optionsManager = FindFirstObjectByType<OptionsManager>();
                    optionsManager.InOptionMenu = true;
                }

                break;
            case InputDeviceChange.Reconnected:
                DeviceStatus($"Gamepad {gamepadReconnectedText}");
                break;
        }
    }

    private void DeviceStatus(string alertMessage)
    {
        if (!Application.isPlaying) // Si no está en modo de juego
            return;

        if (gamepadAlertInstance == null)
        {
            // Obtener una referencia al Canvas que contiene el objeto
            Canvas canvas = FindObjectOfType<Canvas>();

            try
            {
                // Buscar el objeto dentro del Canvas
                gamepadAlertInstance = canvas.transform.Find(gamepadAlertPrefab.name).gameObject;
            }
            catch
            {
                gamepadAlertInstance = Instantiate(gamepadAlertPrefab, canvas.transform);
            }
        }

        gamepadAlertInstance.SetActive(true);
        gamepadAlertInstance.GetComponentInChildren<TMP_Text>().text = alertMessage;
    }


    private void SetDevice(DeviceType newDeviceType)
    {
        if (newDeviceType == DeviceType.Gamepad)
        {
            Cursor.visible = false;
            EventSystem.current.SetSelectedGameObject(selectedButton);
        }
        else if (newDeviceType == DeviceType.KeyboardAndMouse)
        {
            Cursor.visible = true;
            EventSystem.current.SetSelectedGameObject(null);
        }

        this.currentDevice = newDeviceType;
    }

    public void GetCurrentMenuButton(Button button)
    {
        selectedButton = button.gameObject;
        EventSystem.current.SetSelectedGameObject(selectedButton);
    }
}