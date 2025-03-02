using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image loadingImage;
    [SerializeField] private Slider progressBar;

    List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

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

        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
    }

    void Start()
    {
        if (!SceneManager.GetSceneByBuildIndex((int)SceneIndexes.World_SchoolOutdoor).isLoaded)
        {
            SceneManager.LoadSceneAsync((int)SceneIndexes.TitleScreen, LoadSceneMode.Additive);
            SceneManager.LoadSceneAsync((int)SceneIndexes.UI, LoadSceneMode.Additive);
            currentWorldScene = SceneIndexes.TitleScreen;
        }
    }

    [HideInInspector] public SceneIndexes currentWorldScene;
    [HideInInspector] public SceneIndexes previousWorldScene;

    public void LoadScene(SceneIndexes newScene)
    {
        if (currentWorldScene != newScene)
        {
            scenesLoading.Add(SceneManager.UnloadSceneAsync((int)currentWorldScene));
        }

        currentWorldScene = newScene;
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)newScene, LoadSceneMode.Additive));

        StartCoroutine(GetSceneLoadProgress());
    }

    public void LoadBattleScene(SceneIndexes battleScene)
    {
        previousWorldScene = currentWorldScene;
        LoadScene(battleScene);
    }

    private IEnumerator GetSceneLoadProgress()
    {
        LoadingScreenSetActive();

        float totalSceneProgress = 0;

        for (int i = 0; i < scenesLoading.Count; i++)
        {
            while (!scenesLoading[i].isDone)
            {
                totalSceneProgress = 0;

                foreach (AsyncOperation operation in scenesLoading)
                {
                    totalSceneProgress += operation.progress;
                }

                totalSceneProgress = (totalSceneProgress / scenesLoading.Count) * 100;
                progressBar.value = Mathf.RoundToInt(totalSceneProgress);

                yield return null;
            }
        }

        scenesLoading.Clear();

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeOutLoadingScreen());
    }

    private void LoadingScreenSetActive()
    {
        loadingScreen.gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOutLoadingScreen()
    {
        float fadeDuration = 1f;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        loadingScreen.gameObject.SetActive(false);
    }
}
