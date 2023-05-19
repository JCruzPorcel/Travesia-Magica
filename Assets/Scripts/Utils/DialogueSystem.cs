using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI dialogueText;
    [SerializeField] Color playerColor;
    [SerializeField] Color bossColor;
    [SerializeField] Color environmentColor;
    [SerializeField] Image playerImg;
    [SerializeField] GameObject player_Go;
    [SerializeField] GameObject boss_Go;
    [SerializeField] PlayableDirector director;
    [SerializeField] FinalBattleController finalBattleController;

    bool showAnim = false;

    private List<Dialogue> dialogueList = new List<Dialogue>();
    private int currentDialogueIndex = 0;

    private bool isTyping = false;
    [SerializeField] float typingSpeed = 0.1f;

    private void Start()
    {
        LoadDialogue();
        director = GetComponent<PlayableDirector>();
        playerImg.sprite = GameManager.Instance.CharacterSelected.CharacterImage;
    }

    private void Update()
    {
        if (GameManager.Instance.currentGameFlowState == GameFlowState.Boss)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!isTyping)
                {
                    currentDialogueIndex++;
                    if (currentDialogueIndex >= dialogueList.Count)
                    {
                        finalBattleController.StartBossBattle();
                        return;
                    }

                    StartCoroutine(TypeDialogue(dialogueList[currentDialogueIndex]));
                }
                else if (isTyping)
                {
                    StopCoroutine("TypeDialogue");
                    dialogueText.text = dialogueList[currentDialogueIndex].text;
                    isTyping = false;
                }
            }
        }
    }

    private void LoadDialogue()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "dialogues.json");
        string jsonString = File.ReadAllText(filePath);
        DialogueList dialogueData = JsonUtility.FromJson<DialogueList>(jsonString);
        dialogueList = dialogueData.dialogue;
    }

    private IEnumerator TypeDialogue(Dialogue dialogue)
    {
        isTyping = true;

        string currentText = "";
        foreach (char letter in dialogue.text.ToCharArray())
        {
            if (!isTyping)
            {
                break;
            }

            currentText += letter;
            dialogueText.text = currentText;

            player_Go.SetActive(false);
            boss_Go.SetActive(false);

            switch (dialogue.character)
            {
                case "Player":
                    dialogueText.color = playerColor;
                    player_Go.SetActive(true);
                    break;
                case "Boss":
                    dialogueText.color = bossColor;
                    boss_Go.SetActive(true);

                    if (!showAnim)
                    {
                        director.Play();
                        showAnim = true;
                    }

                    break;
                case "Environment":
                    dialogueText.color = environmentColor;
                    break;
                default:
                    break;
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void StartTypeDialogue()
    {
        StartCoroutine(TypeDialogue(dialogueList[currentDialogueIndex]));
    }
}

[System.Serializable]
public class Dialogue
{
    public string character;
    public string text;
}

[System.Serializable]
public class DialogueList
{
    public List<Dialogue> dialogue;
}
