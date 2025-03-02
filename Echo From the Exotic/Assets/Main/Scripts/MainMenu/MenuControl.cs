using UnityEngine;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour
{
    [SerializeField] private Button logoButton;

    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private ModalWindowTemplate[] modalWindowTemplates;

    void Awake()
    {
        logoButton.onClick.AddListener(OnLogoButtonClicked);
        settingsButton.onClick.AddListener(OnSettingButtonClicked);
        creditButton.onClick.AddListener(OnCreditButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnLogoButtonClicked()
    {
        GameManager.instance.LoadScene(SceneIndexes.World_SchoolOutdoor);
    }

    private void OnSettingButtonClicked()
    {

    }

    private void OnCreditButtonClicked()
    {
        currentIndex = 0; // 重置索引
        ShowCurrentModal();
    }

    private void OnExitButtonClicked()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    private int currentIndex = 0;
    private void ShowCurrentModal()
    {
        if (currentIndex >= modalWindowTemplates.Length)
        {
            ModalWindowManager.instance.Close();
            return;
        }

        ModalWindowTemplate currentTemplate = modalWindowTemplates[currentIndex];

        ModalWindowManager.instance.ShowVertical(
            currentTemplate.title,
            currentTemplate.image,
            currentTemplate.context,
            currentTemplate.confirmText, () =>
            {
                currentIndex++;
                ModalWindowManager.instance.Close();
                ShowCurrentModal(); // 顯示下一個
            },
            currentTemplate.declineText, () =>
            {
                ModalWindowManager.instance.Close();
            },
            currentTemplate.alternateText, () =>
            {
                ModalWindowManager.instance.Close();
            }
        );
    }
}
