using TMPro;
using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    public static InteractionPromptUI instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI promptText;

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


    [HideInInspector] public bool IsDisplayed = false;
    public void SetUp(string promptText)
    {
        this.promptText.text = promptText;
        panel.SetActive(true);
        IsDisplayed = true;
    }

    public void Close()
    {
        panel.SetActive(false);
        IsDisplayed = false;
    }
}
