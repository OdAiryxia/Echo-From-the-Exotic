using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    Playing,
    Paused
}

public enum SceneState
{
    Menu,
    World,
    Battle
}

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;
    public GameState currentState = GameState.Playing;
    public SceneState sceneState = SceneState.Menu;

    [SerializeField] private GameObject pauseMenuUI;

    [SerializeField] private Button continueButton;
    [SerializeField] private Button exitBattleButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitGameButton;

    [SerializeField] private ModalWindowTemplate exitBattleTemplate;
    [SerializeField] private ModalWindowTemplate mainMenuTemplate;

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

        Resume();
        MainMenuButtonActive();
        sceneState = SceneState.Menu;
    }

    void Update()
    {
        if (!ModalWindowManager.instance.isWindow)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (sceneState != SceneState.Menu)
                {
                    TogglePause();
                }
            }
        }

        if (currentState == GameState.Paused)
        {
            if (Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
            }

            if (Cursor.visible != true)
            {
                Cursor.visible = true;
            }
        }
    }

    public void TogglePause()
    {
        switch (currentState)
        {
            case GameState.Playing:
                Pause();
                break;

            case GameState.Paused:
                Resume();
                break;
        }
    }

    public void Pause()
    {
        pauseMenuUI?.SetActive(true);
        Time.timeScale = 0f;
        currentState = GameState.Paused;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        pauseMenuUI?.SetActive(false);
        Time.timeScale = 1f;
        currentState = GameState.Playing;

        if (sceneState != SceneState.Battle)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }

    public void ExitBattleButtonActive()
    {
        mainMenuButton.gameObject.SetActive(false);
        exitBattleButton.gameObject.SetActive(true);
        sceneState = SceneState.Battle;
    }

    public void MainMenuButtonActive()
    {
        exitBattleButton.gameObject.SetActive(false);
        mainMenuButton.gameObject.SetActive(true);
        sceneState = SceneState.World;
    }

    public void ContinueButton()
    {
        Resume();
    }

    public void ExitBattleButton()
    {
        ShowCurrentModalExitBattle();
    }

    public void MainMenuButton()
    {
        ShowCurrentModalMainMenu();
    }

    public void QuitGameButton()
    {
        Application.Quit();
    }

    private void ShowCurrentModalMainMenu()
    {
        ModalWindowTemplate currentTemplate = mainMenuTemplate;

        ModalWindowManager.instance.ShowVertical(
            currentTemplate.title,
            currentTemplate.image,
            currentTemplate.context,
            currentTemplate.confirmText, () =>
            {
                Resume();
                GameManager.instance.LoadScene((int)SceneIndexes.TitleScreen);
                sceneState = SceneState.Menu;
                BlackScreenManager.instance.StartCoroutine(BlackScreenManager.instance.FadeOut());
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

    private void ShowCurrentModalExitBattle()
    {
        ModalWindowTemplate currentTemplate = mainMenuTemplate;

        ModalWindowManager.instance.ShowVertical(
            currentTemplate.title,
            currentTemplate.image,
            currentTemplate.context,
            currentTemplate.confirmText, () =>
            {
                Resume();
                BattleManager.instance.EndBattle(false);
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
