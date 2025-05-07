using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;
using System.Collections;

public class ModalWindowManager : MonoBehaviour
{
    public static ModalWindowManager instance;
    public bool isWindow;

    [SerializeField] private Transform panel;
    [SerializeField] private Transform modalWindow;

    [Header("Header")]
    [SerializeField] private Transform headerArea;
    [SerializeField] private TextMeshProUGUI headerTitle;

    [Header("Content")]
    [SerializeField] private Transform contentArea;
    [Space(5)]
    [SerializeField] private Transform verticalLayoutArea;
    [SerializeField] private Image verticalImage;
    [SerializeField] private TextMeshProUGUI verticalContent;
    [Space(5)]
    [SerializeField] private Transform horizontalLayoutArea;
    [SerializeField] private Image horizontalImage;
    [SerializeField] private TextMeshProUGUI horizontalContent;
    [Space(5)]
    [SerializeField] private Transform inputFieldLayoutArea;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI inputFieldContent;

    [Header("Footer")]
    [SerializeField] private Transform footerArea;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TextMeshProUGUI confirmButtonText;
    [SerializeField] private Button declineButton;
    [SerializeField] private TextMeshProUGUI declineButtonText;
    [SerializeField] private Button alternateButton;
    [SerializeField] private TextMeshProUGUI alternateButtonText;

    private Action onConfirmAction;
    private Action onDeclineAction;
    private Action onAlternateAction;

    private CharacterMovement player;

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
        isWindow = false;
        panel.gameObject.SetActive(false);
        modalWindow.gameObject.SetActive(false);
    }

    public void Close()
    {
        isWindow = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;

        panel.gameObject.SetActive(false);
        modalWindow.gameObject.SetActive(false);
        ProgressManager.instance.isStory = false;
    }

    public void ShowVertical(string title, Sprite image, string content, string confirmText, Action confirmAction, string declineText = null, Action declineAction = null, string alternateText = null, Action alternateAction = null)
    {
        isWindow = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        panel.gameObject.SetActive(true);
        modalWindow.gameObject.SetActive(true);
        inputFieldLayoutArea.gameObject.SetActive(false);
        horizontalLayoutArea.gameObject.SetActive(false);
        verticalLayoutArea.gameObject.SetActive(true);

        confirmButton.onClick.RemoveAllListeners();
        declineButton.onClick.RemoveAllListeners();
        alternateButton.onClick.RemoveAllListeners();

        // 設置標題區域
        bool hasTitle = !string.IsNullOrEmpty(title);
        headerArea.gameObject.SetActive(hasTitle);
        headerTitle.text = title;

        // 設置圖片
        bool hasImage = (image != null);
        verticalImage.gameObject.SetActive(hasImage);
        if (hasImage)
        {
            verticalImage.sprite = image;
        }
        else
        {
            verticalImage.sprite = null;
        }

        // 設置內容文字
        verticalContent.text = content;

        // 設置確認按鈕
        confirmButton.gameObject.SetActive(true);
        confirmButtonText.text = confirmText;
        confirmButton.onClick.AddListener(new UnityAction(confirmAction));

        // 設置拒絕按鈕
        bool hasDecline = !string.IsNullOrEmpty(declineText);
        declineButton.gameObject.SetActive(hasDecline);
        if (hasDecline)
        {
            if (declineText != null)
            {
                declineButtonText.text = declineText;
            }
            declineButton.onClick.AddListener(new UnityAction(declineAction));
        }

        // 設置備選按鈕
        bool hasAlternate = !string.IsNullOrEmpty(alternateText);
        alternateButton.gameObject.SetActive(hasAlternate);
        if (hasAlternate)
        {
            if (alternateText != null)
            {
                alternateButtonText.text = alternateText;
            }
            alternateButton.onClick.AddListener(new UnityAction(alternateAction));
        }

        Time.timeScale = 0f;

        ProgressManager.instance.isStory = true;
        StartCoroutine(DelayedShow());
    }

    public void ShowHorizontal(string title, Sprite image, string content, string confirmText, Action confirmAction, string declineText = null, Action declineAction = null, string alternateText = null, Action alternateAction = null)
    {
        isWindow = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        CharacterMovement[] players = FindObjectsByType<CharacterMovement>(FindObjectsSortMode.None);
        player = players[0];
        player.freeLookCamera.m_XAxis.m_InputAxisName = "";
        player.freeLookCamera.m_YAxis.m_InputAxisName = "";

        modalWindow.gameObject.SetActive(true);
        inputFieldLayoutArea.gameObject.SetActive(false);
        verticalLayoutArea.gameObject.SetActive(false);
        horizontalLayoutArea.gameObject.SetActive(true);

        confirmButton.onClick.RemoveAllListeners();
        declineButton.onClick.RemoveAllListeners();
        alternateButton.onClick.RemoveAllListeners();

        // 設置標題區域
        bool hasTitle = !string.IsNullOrEmpty(title);
        headerArea.gameObject.SetActive(hasTitle);
        headerTitle.text = title;

        // 設置圖片
        bool hasImage = (image != null);
        horizontalImage.gameObject.SetActive(hasImage);
        if (hasImage)
        {
            horizontalImage.sprite = image;
        }
        else
        {
            horizontalImage.sprite = null;
        }

        // 設置內容文字
        horizontalContent.text = content;

        // 設置確認按鈕
        confirmButton.gameObject.SetActive(true);
        confirmButtonText.text = confirmText;
        confirmButton.onClick.AddListener(new UnityAction(confirmAction));

        // 設置拒絕按鈕
        bool hasDecline = !string.IsNullOrEmpty(declineText);
        declineButton.gameObject.SetActive(hasDecline);
        if (hasDecline)
        {
            if (declineText != null)
            {
                declineButtonText.text = declineText;
            }
            declineButton.onClick.AddListener(new UnityAction(declineAction));
        }

        // 設置備選按鈕
        bool hasAlternate = !string.IsNullOrEmpty(alternateText);
        alternateButton.gameObject.SetActive(hasAlternate);
        if (hasAlternate)
        {
            if (alternateText != null)
            {
                alternateButtonText.text = alternateText;
            }
            alternateButton.onClick.AddListener(new UnityAction(alternateAction));
        }

        ProgressManager.instance.isStory = true;
        StartCoroutine(DelayedShow());
    }

    public void ShowInputField()
    {
        isWindow = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        panel.gameObject.SetActive(true);
        modalWindow.gameObject.SetActive(true);
        horizontalLayoutArea.gameObject.SetActive(false);
        verticalLayoutArea.gameObject.SetActive(false);
        inputFieldLayoutArea.gameObject.SetActive(true);

        confirmButton.onClick.RemoveAllListeners();
        declineButton.onClick.RemoveAllListeners();
        alternateButton.onClick.RemoveAllListeners();

        // 設置標題區域
        headerTitle.text = "輸入你的名稱";

        inputFieldContent.text = "";
        inputFieldContent.color = new Color (inputFieldContent.color.r, inputFieldContent.color.g, inputFieldContent.color.b, 0f);
        inputField.text = null;

        // 設置確認按鈕
        confirmButton.gameObject.SetActive(true);
        confirmButtonText.text = "確認";
        confirmButton.onClick.AddListener(CheckName);

        declineButton.gameObject.SetActive(false);
        alternateButton.gameObject.SetActive(false);

        Time.timeScale = 0f;

        ProgressManager.instance.isStory = true;
        StartCoroutine(DelayedShow());
    }


    private IEnumerator DelayedShow()
    {
        yield return null; // 等待一幀

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)modalWindow);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)horizontalLayoutArea);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)verticalLayoutArea);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)inputFieldLayoutArea);

        yield return null; // 等待一幀

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)modalWindow);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)horizontalLayoutArea);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)verticalLayoutArea);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)inputFieldLayoutArea);
    }

    void CheckName()
    {
        if (inputField.text.Length >= 15)
        {
            inputFieldContent.text = "名稱長度過長";
            inputFieldContent.color = new Color(inputFieldContent.color.r, inputFieldContent.color.g, inputFieldContent.color.b, 1f);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)inputFieldLayoutArea);
        }
        else if (string.IsNullOrEmpty(inputField.text))
        {
            inputFieldContent.text = "名稱不得為空白";
            inputFieldContent.color = new Color(inputFieldContent.color.r, inputFieldContent.color.g, inputFieldContent.color.b, 1f);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)inputFieldLayoutArea);
        }
        else if (!string.IsNullOrEmpty(inputField.text))
        {
            inputFieldContent.text = null;
            inputFieldContent.color = new Color(inputFieldContent.color.r, inputFieldContent.color.g, inputFieldContent.color.b, 0f);
            DataContainer.instance.playerName = inputField.text;
            ProgressManager.instance.SetPlayerName();
            Close();
        }
    }
}
