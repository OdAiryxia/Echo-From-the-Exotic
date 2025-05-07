using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    #region Initialize
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (!SceneManager.GetSceneByBuildIndex((int)SceneIndexes.UI).isLoaded)
        {
            SceneManager.LoadSceneAsync((int)SceneIndexes.UI, LoadSceneMode.Additive);
        }

        SceneManager.LoadSceneAsync((int)SceneIndexes.TitleScreen, LoadSceneMode.Additive);
        currentWorld = (int)SceneIndexes.TitleScreen;
        previousWorld = (int)WorldIndexes.world_01_classroom;
        Initialize();
    }

    void Initialize()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    #endregion

    [HideInInspector] public int currentWorld;
    [HideInInspector] public int previousWorld;

    public PlayerData playerData = new PlayerData();
    private GameObject currentPlayer;

    private List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

    public void LoadScene(int scene)
    {
        BlackScreenManager.instance.blackScreen.color = new Color(BlackScreenManager.instance.blackScreen.color.r, BlackScreenManager.instance.blackScreen.color.g, BlackScreenManager.instance.blackScreen.color.b, 1f);
        if (currentWorld != scene)
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentWorld);
            if (unloadOp != null)
            {
                scenesLoading.Add(unloadOp);
            }
        }

        currentWorld = scene;

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        if (loadOp != null)
        {
            scenesLoading.Add(loadOp);
        }
    }


    public void LoadWorld(WorldIndexes scene, string spawnpointID)
    {
        if (currentPlayer != null)
        {
            playerData.scene = currentWorld;
            playerData.position = currentPlayer.transform.position;
            playerData.rotation = currentPlayer.transform.rotation;
        }

        LoadScene((int)scene);
        StartCoroutine(SpawnPlayer(spawnpointID));
        PauseManager.instance.MainMenuButtonActive();
    }

    public void LoadBattle(BattlefieldIndexes scene)
    {
        previousWorld = currentWorld;
        if (currentPlayer != null)
        {
            playerData.position = currentPlayer.transform.position;
            playerData.rotation = currentPlayer.transform.rotation;
        }

        LoadScene((int)scene);
        StartCoroutine(WaitForBattleLoad());
        PauseManager.instance.ExitBattleButtonActive();
    }

    IEnumerator SpawnPlayer(string spawnpointID)
    {
        if (scenesLoading.Count > 0)
        {
            while (!scenesLoading.TrueForAll(op => op != null && op.isDone))
                yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < 20; i++)
        {
            CharacterMovement[] players = FindObjectsByType<CharacterMovement>(FindObjectsSortMode.None);
            if (players.Length > 0)
            {
                currentPlayer = players[0].gameObject;
                break;
            }
            yield return null;
        }

        if (currentPlayer == null)
        {
            Debug.LogError("❌ SpawnPlayer 找不到玩家，請檢查場景是否正確生成角色！");
            yield break;
        }

        yield return null;

        bool positionSet = false;

        for (int i = 0; i < 20; i++)
        {
            if (!string.IsNullOrEmpty(spawnpointID))
            {
                Spawnpoint[] spawnpoints = FindObjectsByType<Spawnpoint>(FindObjectsSortMode.None);
                foreach (var sp in spawnpoints)
                {
                    if (sp.spawnpointID == spawnpointID)
                    {
                        currentPlayer.GetComponent<CharacterController>().enabled = false;
                        currentPlayer.transform.position = sp.transform.position;
                        currentPlayer.transform.rotation = sp.transform.rotation;
                        currentPlayer.GetComponent<CharacterController>().enabled = true;
                        positionSet = true;
                        break;
                    }
                }
            }
            else if (playerData.scene == currentWorld)
            {
                currentPlayer.transform.position = playerData.position;
                currentPlayer.transform.rotation = playerData.rotation;
                positionSet = true;
            }
            else
            {
                currentPlayer.transform.position = Vector3.zero;
                positionSet = true;
            }

            if (positionSet) break;
            yield return null;
        }

        if (!positionSet)
        {
            Debug.LogError("❌ 玩家位置未能成功設置！");
        }

        BlackScreenManager.instance.StartCoroutine(BlackScreenManager.instance.FadeOut());
    }

    IEnumerator WaitForBattleLoad()
    {
        while (!scenesLoading.TrueForAll(op => op.isDone))
            yield return null;

        Debug.Log("戰鬥場景載入完成！");
        BlackScreenManager.instance.StartCoroutine(BlackScreenManager.instance.FadeOut());
    }
}
