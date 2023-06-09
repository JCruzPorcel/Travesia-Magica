using TMPro;
using UnityEngine;

public class cm_ScoreAdd : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GameManager.Instance.currentGameState == GameState.InGame)
        {
            animator.speed = 1f;
        }
        else
        {
            animator.speed = 0f;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ScoreManager scoreManager = ScoreManager.Instance;
        TextMeshProUGUI scoreText = animator.GetComponent<TextMeshProUGUI>();

        if (long.TryParse(scoreText.text, out long score))
        {
            scoreManager.AddScore(score);
            scoreText.text = string.Empty;
        }
        else
        {
            //Debug.LogError($"Unable to parse score: {scoreText.text}");
        }
    }


    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
