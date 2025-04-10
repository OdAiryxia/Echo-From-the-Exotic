using TMPro;
using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    public static InteractionPromptUI instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI promptText;

    [HideInInspector] public bool IsDisplayed = false;

    // 加上目前的 prompt 記錄
    public string CurrentPrompt { get; private set; }

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
        panel.SetActive(false);
    }

    public void SetUp(string prompt)
    {
        // 只在內容不同時才更新文字
        if (CurrentPrompt != prompt)
        {
            promptText.text = prompt;
            CurrentPrompt = prompt;
        }
        panel.SetActive(true);
        IsDisplayed = true;
    }

    public void Close()
    {
        panel.SetActive(false);
        IsDisplayed = false;
        CurrentPrompt = null;
    }
}
