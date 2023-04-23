using UnityEngine;

public class AnimatorLayerSelector : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] string layerName;

    void Start()
    {
        if(animator == null)
            animator = GetComponent<Animator>();

        int layerIndex = animator.GetLayerIndex(layerName);

        if (layerIndex == -1)
        {
            Debug.LogError("Layer not found in Animator.");
            return;
        }

        animator.SetLayerWeight(layerIndex, 1f);
    }
}
