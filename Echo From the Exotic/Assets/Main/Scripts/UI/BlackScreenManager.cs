using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreenManager : MonoBehaviour
{
    public static BlackScreenManager instance;
    public Image blackScreen;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 1f);
        StartCoroutine(FadeOut());
    }

    public IEnumerator FadeIn()
    {
        float duration = 2f;
        float elapsedTime = 0f;

        Color startColor = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 0f);
        Color targetColor = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 1f);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);

            blackScreen.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        blackScreen.color = targetColor;
    }

    public IEnumerator FadeOut()
    {
        float duration = 2f;
        float elapsedTime = 0f;

        Color startColor = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 1f);
        Color targetColor = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 0f);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);

            blackScreen.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        blackScreen.color = targetColor;
    }

    public IEnumerator Transition(CanvasGroup canvasGroup)
    {
        yield return new WaitForSeconds(2);

        yield return StartCoroutine(FadeIn());
        yield return new WaitForSeconds(0.2f);

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        yield return StartCoroutine(FadeOut());
    }
}
