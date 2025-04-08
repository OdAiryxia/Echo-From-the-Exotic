using UnityEngine;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour
{
    [SerializeField] private Button logoButton;

    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private ModalWindowTemplate modalWindowTemplates;

    public void OnLogoButtonClicked()
    {
        GameManager.instance.LoadWorld(WorldIndexes.world_01_classroom, "hallway");
    }

    public void OnSettingButtonClicked()
    {

    }

    public void OnCreditButtonClicked()
    {
        ShowCurrentModal();
    }

    public void OnExitButtonClicked()
    {
        Application.Quit();
    }

    private void ShowCurrentModal()
    {
        ModalWindowTemplate currentTemplate = modalWindowTemplates;

        ModalWindowManager.instance.ShowVertical(
            currentTemplate.title,
            currentTemplate.image,
            currentTemplate.context,
            currentTemplate.confirmText, () =>
            {
                ModalWindowManager.instance.Close();
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
