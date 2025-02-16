using UnityEngine;
using TMPro;

public class LoadingScreenText : MonoBehaviour
{
    private TextMeshProUGUI text;
    private float changeInterval = 1f;

    private string baseText = "Now Loading";
    private int dotCount = 0;
    private float timer = 0f;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= changeInterval)
        {
            timer = 0f;
            dotCount = (dotCount + 1) % 4;
            text.text = baseText + new string('.', dotCount);
        }
    }
}
