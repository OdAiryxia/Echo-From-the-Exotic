using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    public static TypewriterEffect instance;
    public TextMeshProUGUI text;

    public float delay = 0.1f;
    public float fadeOutTime = 3f;
    public bool isFading = false;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        StartCoroutine(ShowText(""));
    }

    public IEnumerator ShowText(string newText)
    {
        StopCoroutine(FadeText());
        isFading = false;

        text.text = newText;
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
        text.ForceMeshUpdate();

        for (int i = 0; i <= text.text.Length; i++)
        {
            text.maxVisibleCharacters = i;
            yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(fadeOutTime);
        StartCoroutine(FadeText());
    }

    IEnumerator FadeText()
    {
        isFading = true;
        Color originalColor = text.color;

        for (float t = 0f; t < 2f; t += Time.deltaTime)
        {
            if (!isFading) yield break;

            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(originalColor.a, 0f, t / 2f));
            yield return null;
        }
        text.ForceMeshUpdate();
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        isFading = false;
    }
}

