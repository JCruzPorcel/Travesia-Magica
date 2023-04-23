using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MenuManager : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuGo;
    [Space(5)]
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button exitButton;

    public Button PlayButton { get { return playButton; } }


    [Space(10)]

    [Header("Character Selection")]
    [SerializeField] private GameObject charSelectionGo;

    [Space(5)]

    [Header("Character One")]
    [SerializeField] private Button characterOneButton;
    [SerializeField] private Image characterOneImage;
    [SerializeField] private GameObject CharOne_Panel;
    [SerializeField] private CharacterData characterOne;

    [Space(10)]

    [Header("Character Two")]
    [SerializeField] private Button characterTwoButton;
    [SerializeField] private Image characterTwoImage;
    [SerializeField] private GameObject CharTwo_Panel;
    [SerializeField] private CharacterData characterTwo;

    [Space(10)]

    [Header("Confirm Character")]
    [SerializeField] private GameObject confirmCharacterGo;
    [SerializeField] private Button confirmCharacterButton;
    [Space(5)]
    [SerializeField] private string nextText;
    [SerializeField] private string confirmText;
    private bool canStartGame = false;

    private GameObject lastPanel;
    private Button lastButton;

    [Space(10)]

    [Header("Partners")]
    [SerializeField] private PetData petOne;
    [SerializeField] private PetData petTwo;


    [Space(10)]

    [Header("Options")]
    [SerializeField] private GameObject optionGo;
    [Space(5)]
    [SerializeField] private Button backToMenuButton;

    [Space(10)]

    [Header("Load Level")]
    [SerializeField] private string levelName = string.Empty;
    public string LevelName { get { return levelName; } }

    [SerializeField] private float loadingTime = 0f;

    [SerializeField] FadeInOut fadeInOut;
    [SerializeField] private float transitionTime = 1f;

    GameManager gameManager;
    DeviceManager deviceManager;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        deviceManager = gameManager.GetComponent<DeviceManager>();

        if (gameManager.currentGameState == GameState.MainMenu)
            CloseWindows();

        deviceManager.CheckSelectedButtonIsNull();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;

        characterOneImage.sprite = characterOne.CharacterImage;
        characterTwoImage.sprite = characterTwo.CharacterImage;

        if (confirmCharacterGo != null)
        {
            confirmCharacterGo.SetActive(false);
        }

        if (CharOne_Panel != null && CharTwo_Panel != null)
        {
            CharOne_Panel.SetActive(false);
            CharTwo_Panel.SetActive(false);
        }

        #region Button Listener

        playButton.onClick.AddListener(() =>
        {
            StartCoroutine(MenuTransition(charSelectionGo, mainMenuGo, false));
            SelectedNewButton(characterOneButton);
        });

        optionButton.onClick.AddListener(() =>
        {
            StartCoroutine(MenuTransition(optionGo, mainMenuGo, false));

            Button button = FindFirstObjectByType<OptionsManager>().BackButton;
            SelectedNewButton(button);
        });

        exitButton.onClick.AddListener(() =>
        {
            DisableButtonInteraction();
            gameManager.ExitGame(transitionTime);
        });

        backToMenuButton.onClick.AddListener(OnBackToMenuClicked);

        characterOneButton.onClick.AddListener(() =>
        {
            if (canStartGame)
            {
                Select(petOne, CharOne_Panel, confirmText);
            }
            else
            {
                Select(characterOne, CharOne_Panel, nextText);
            }

            SelectedNewButton(confirmCharacterButton);
        });

        characterTwoButton.onClick.AddListener(() =>
        {
            if (canStartGame)
            {
                Select(petTwo, CharTwo_Panel, confirmText);
            }
            else
            {
                Select(characterTwo, CharTwo_Panel, nextText);
            }

            SelectedNewButton(confirmCharacterButton);
        });

        confirmCharacterButton.onClick.AddListener(HandleConfirmButtonClicked);

        #endregion
    }

    void OnBackToMenuClicked()
    {
        if (!canStartGame)
        {
            StartCoroutine(MenuTransition(mainMenuGo, charSelectionGo, false));
            SelectedNewButton(playButton);
        }
        else
        {
            canStartGame = false;
            SetCharacterImages();
            SetConfirmCharacterText();
            ShowPanels();
            SelectedNewButton(lastButton);
        }
    }

    void SetCharacterImages()
    {
        characterOneImage.sprite = characterOne.CharacterImage;
        characterTwoImage.sprite = characterTwo.CharacterImage;
    }

    void SetConfirmCharacterText()
    {
        confirmCharacterGo.GetComponentInChildren<TMP_Text>().text = nextText;
    }

    void ShowPanels()
    {
        CharOne_Panel.SetActive(false);
        CharTwo_Panel.SetActive(false);
        lastPanel.SetActive(true);
    }

    private void HandleConfirmButtonClicked()
    {
        if (canStartGame)
        {
            StartCoroutine(StartGameTransition());
        }
        else
        {
            HideCharacterPanels();
            HideConfirmCharacter();
            SetPetImages();
            SelectedNewButton(characterOneButton);
        }

        canStartGame = true;
    }

    private IEnumerator StartGameTransition()
    {
        yield return StartCoroutine(MenuTransition(null, null, true));
        gameManager.StartGame(loadingTime);
    }

    private void HideCharacterPanels()
    {
        if (CharOne_Panel != null && CharTwo_Panel != null)
        {
            CharOne_Panel.SetActive(false);
            CharTwo_Panel.SetActive(false);
        }
    }

    private void HideConfirmCharacter()
    {
        confirmCharacterGo.SetActive(false);
    }

    private void SetPetImages()
    {
        characterOneImage.sprite = petOne.PetImage;
        characterTwoImage.sprite = petTwo.PetImage;
    }


    private void WindowManagement(GameObject openMenu, GameObject closeMenu)
    {
        if (openMenu != null && closeMenu != null)
        {
            openMenu.SetActive(true);
            closeMenu.SetActive(false);
        }

        if (CharOne_Panel != null && CharTwo_Panel != null)
        {
            CharOne_Panel.SetActive(false);
            CharTwo_Panel.SetActive(false);
        }
    }

    private void Select<T>(T data, GameObject panel, string text)
    {
        if (data != null)
        {
            if (data is PetData pet)
            {
                gameManager.PetSelected = pet;
            }
            else if (data is CharacterData character)
            {
                lastPanel = panel;
                lastButton = lastPanel.GetComponentInParent<Button>();
                gameManager.CharacterSelected = character;
            }
        }

        if (confirmCharacterGo != null)
        {
            confirmCharacterGo.SetActive(true);
            confirmCharacterGo.GetComponentInChildren<TMP_Text>().text = text;
        }

        if (CharOne_Panel != null && CharTwo_Panel != null)
        {
            CharOne_Panel.SetActive(false);
            CharTwo_Panel.SetActive(false);
        }

        if (panel != null)
        {
            panel.SetActive(true);
        }
    }

    IEnumerator MenuTransition(GameObject openMenu, GameObject closeMenu, bool isLoading)
    {
        fadeInOut.FadeIn();
        fadeInOut.IsLoading = isLoading;
        DisableButtonInteraction();

        yield return new WaitForSeconds(transitionTime);

        if (isLoading)
        {
            gameManager.CurrentLevelName = levelName;
            gameManager.LoadLevelAsync(loadingTime, levelName);
        }
        else
        {
            if (openMenu != null && closeMenu != null)
            {
                WindowManagement(openMenu, closeMenu);
                fadeInOut.FadeOut();
            }
        }

        if (confirmCharacterGo != null && confirmCharacterGo.activeInHierarchy)
        {
            confirmCharacterGo.SetActive(false);
        }

        EnableButtonInteraction();
    }

    private void CloseWindows()
    {
        mainMenuGo.SetActive(true);
        optionGo.SetActive(false);
        charSelectionGo.SetActive(false);
    }

    private void DisableButtonInteraction()
    {
        backToMenuButton.interactable = false;
        characterOneButton.interactable = false;
        playButton.interactable = false;
        optionButton.interactable = false;
        exitButton.interactable = false;
        confirmCharacterButton.interactable = false;
        characterTwoButton.interactable = false;
        characterOneButton.interactable = false;
    }

    private void EnableButtonInteraction()
    {
        backToMenuButton.interactable = true;
        characterOneButton.interactable = true;
        playButton.interactable = true;
        optionButton.interactable = true;
        exitButton.interactable = true;
        confirmCharacterButton.interactable = true;
        characterTwoButton.interactable = true;
        characterOneButton.interactable = true;
    }

    private void SelectedNewButton(Button button)
    {
        deviceManager.GetCurrentMenuButton(button);
    }
}
