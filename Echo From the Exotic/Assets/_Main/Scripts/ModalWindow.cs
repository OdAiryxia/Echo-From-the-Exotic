using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

public class ModalWindow : MonoBehaviour
{
    public static ModalWindow Instance;

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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        panel.gameObject.SetActive(false);
        modalWindow.gameObject.SetActive(false);
    }

    public void Close()
    {
        panel.gameObject.SetActive(false);
        modalWindow.gameObject.SetActive(false);
    }

    public void ShowVertical(string title, Sprite image, string content, string confirmText, Action confirmAction, string declineText = null, Action declineAction = null, string alternateText = null, Action alternateAction = null)
    {
        panel.gameObject.SetActive(true);
        modalWindow.gameObject.SetActive(true);
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

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)modalWindow);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)modalWindow);
    }

    public void ShowHorizontal(string title, Sprite image, string content, string confirmText, Action confirmAction, string declineText = null, Action declineAction = null, string alternateText = null, Action alternateAction = null)
    {
        modalWindow.gameObject.SetActive(true);
        horizontalLayoutArea.gameObject.SetActive(true);
        verticalLayoutArea.gameObject.SetActive(false);

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

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)modalWindow);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)modalWindow);
    }
}
