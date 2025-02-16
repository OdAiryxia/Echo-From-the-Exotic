using UnityEngine;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour
{
    [SerializeField] private Button logoButton;

    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditButton;
    [SerializeField] private Button exitButton;

    void Awake()
    {
        logoButton.onClick.AddListener(OnLogoButtonClicked);
        settingsButton.onClick.AddListener(OnSettingButtonClicked);
        creditButton.onClick.AddListener(OnCreditButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnLogoButtonClicked()
    {
        GameManager.instance.EnterGame();
    }

    private void OnSettingButtonClicked()
    {

    }

    private void OnCreditButtonClicked()
    {

    }

    private void OnExitButtonClicked()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
