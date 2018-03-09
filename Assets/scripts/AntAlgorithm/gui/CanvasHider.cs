/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2018
 *****************************************************/

/* CanvasHider is dragged in to the Scene and is used for hiding ui elements*/
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CanvasHider : MonoBehaviour
{
    public Button toggleCanvasButton;
    public Canvas myCanvas;
    public bool toggleFlag = false;
    public float fadeSpeed;
    void Start()
    {
        fadeSpeed = Time.deltaTime * 2;
        toggleCanvasButton.onClick.AddListener(Toggle);
    }

    void Toggle()
    {
        if (!toggleFlag)
        {
            StartCoroutine(FadeOut(myCanvas));
            toggleCanvasButton.GetComponentInChildren<Text>().text = "v";
            toggleFlag = true;
        }
        else
        {
            StartCoroutine(FadeIn(myCanvas));
            toggleCanvasButton.GetComponentInChildren<Text>().text = "^";
            toggleFlag = false;
        }
    }
    public void Hide()
    {
        StartCoroutine(FadeOut(myCanvas));
        toggleCanvasButton.GetComponentInChildren<Text>().text = "v";
        toggleFlag = true;

    }

    public void Show()
    {
        StartCoroutine(FadeIn(myCanvas));
        toggleCanvasButton.GetComponentInChildren<Text>().text = "^";
        toggleFlag = false;
    }

    IEnumerator FadeOut(Canvas canvas)
    {
        CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= fadeSpeed;
            yield return null;
        }
        canvasGroup.interactable = false;
        yield return null;
    }

    IEnumerator FadeIn(Canvas canvas)
    {
        CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += fadeSpeed;
            yield return null;
        }
        canvasGroup.interactable = true;
        yield return null;
    }
}
