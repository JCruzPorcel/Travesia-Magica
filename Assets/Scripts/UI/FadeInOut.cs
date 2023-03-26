using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{

    [Tooltip("duración del fade en segundos")][SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float startAlpha = 0f;
    [SerializeField] private float endAlpha = 1f;
    [SerializeField] private bool isLoading = false;

    public bool IsLoading { get => isLoading; set => isLoading = value; }

    [SerializeField] private GameObject loadingScreen;
    private CanvasGroup canvasGroup;
    [SerializeField] private Animator animator;
    private int loopCount;
    [SerializeField] Sprite finsihLoadImg;


    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = endAlpha;

        FadeOut();
    }

    void Update()
    {
        if (isLoading)
        {
            if (loadingScreen.activeInHierarchy)
            {
                float normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                if (normalizedTime >= 1.0f)
                {
                    loopCount++;
                    animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, 0f);
                }

                if (loopCount >= Random.Range(3,5))
                {
                    animator.speed = 0f;
                    Image img = animator.GetComponent<Image>();
                    img.sprite = finsihLoadImg;
                }
            }
        }
    }

    public void FadeOut()
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, endAlpha, startAlpha, fadeDuration));
    }

    public void FadeIn()
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, startAlpha, endAlpha, fadeDuration));
    }

    IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            canvasGroup.alpha = alpha;
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = endAlpha;

        loadingScreen.SetActive(isLoading);
    }
}
