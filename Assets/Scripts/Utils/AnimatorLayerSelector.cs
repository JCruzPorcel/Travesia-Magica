using UnityEngine;

public class AnimatorLayerSelector : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] string layerName;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        int layerIndex = animator.GetLayerIndex(layerName);

        if (layerIndex == -1)
        {
            Debug.LogError("Layer not found in Animator.");
            return;
        }

        for (int i = 0; i < animator.layerCount; i++)
        {
            if (i != layerIndex)
            {
                animator.SetLayerWeight(i, 0f);
            }
        }

        animator.SetLayerWeight(layerIndex, 1f);
    }
}
