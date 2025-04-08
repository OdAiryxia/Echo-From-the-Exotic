using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractorDialog : MonoBehaviour
{
    public static InteractorDialog instance;
    public TextMeshProUGUI text;

    public float delay = 0.1f;
    public float fadeOutTime = 3f;
    private Coroutine typeCoroutine;
    private Coroutine fadeCoroutine;

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
    }

    void Start()
    {
        StartTyping("");
    }

    public void StartTyping(string newText)
    {
        // 確保不會有多個 coroutine 同時執行
        if (typeCoroutine != null) StopCoroutine(typeCoroutine);
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        typeCoroutine = StartCoroutine(ShowText(newText));
    }

    private IEnumerator ShowText(string newText)
    {
        text.text = newText;
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
        text.maxVisibleCharacters = 0;
        text.ForceMeshUpdate();

        for (int i = 0; i <= text.text.Length; i++)
        {
            text.maxVisibleCharacters = i;
            yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(fadeOutTime);
        fadeCoroutine = StartCoroutine(FadeText());
    }

    private IEnumerator FadeText()
    {
        Color originalColor = text.color;
        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, 0f, elapsed / duration);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }
}
