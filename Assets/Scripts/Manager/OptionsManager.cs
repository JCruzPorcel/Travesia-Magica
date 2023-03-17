using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] Button backButton;
    [SerializeField] GameObject optionMenu;
    [SerializeField] GameObject mainMenu;
    bool inOptionMenu;
    FadeInOut fadeInOut;
    float transitionTime = 1f;

    void Start()
    {
        fadeInOut = FindAnyObjectByType<FadeInOut>();


        backButton.onClick.AddListener(() =>
        {
            CloseOptionMenu();
        });
    }


    public void CloseOptionMenu()
    {
        StartCoroutine(MenuTransition(optionMenu, mainMenu));
    }

    IEnumerator MenuTransition(GameObject optionMenu, GameObject otherMenu)
    {
        backButton.interactable = false;
        inOptionMenu = !inOptionMenu;
        fadeInOut.FadeIn();

        yield return new WaitForSeconds(transitionTime);

        if (otherMenu != null)
        {
            if (inOptionMenu)
            {
                optionMenu.SetActive(false);
                otherMenu.SetActive(true);
            }
            else
            {
                optionMenu.SetActive(false);
                otherMenu.SetActive(true);
            }
        }
        else
        {
            optionMenu.SetActive(!inOptionMenu);
        }

        fadeInOut.FadeOut();
        backButton.interactable = true;
    }
}
