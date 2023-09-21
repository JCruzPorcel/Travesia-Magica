using UnityEngine;

public class FinalBattleController : MonoBehaviour
{
    [SerializeField] GameObject textBox;
    [SerializeField] DialogueSystem dialogueSystem;
    [SerializeField] GameObject bossGo;
    [SerializeField] GameObject iconBoss;

    private void Start()
    {
        //GameManager.OnGameStateChanged += HandleGameStateChanged;
        GameManager.OnGameFlowStateChanged += HandleGameFlowChanged;
    }

    /*private void HandleGameStateChanged(GameState newGameState)
    {

    }*/



    private void HandleGameFlowChanged(GameFlowState newGameFlow)
    {
        if (newGameFlow == GameFlowState.Boss)
        {
            textBox.SetActive(true);
            dialogueSystem.StartTypeDialogue();
        }
    }

    public void StartBossBattle()
    {
        bossGo.SetActive(true);
        iconBoss.SetActive(false);
        textBox.SetActive(false);
    }
}
