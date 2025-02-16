using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private GameObject loadingScreen;

    [SerializeField] private Image loadingImage;
    [SerializeField] private Slider progressBar;

    public bool reEnterGame = false;

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

        if (reEnterGame)
        {
            SceneManager.UnloadSceneAsync((int)SceneIndexes.World_SchoolOutdoor);
            SceneManager.LoadSceneAsync((int)SceneIndexes.TitleScreen, LoadSceneMode.Additive);
        }
    }

    List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
    public void EnterGame()
    {
        LoadingScreenSetActive();

        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.TitleScreen));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.World_SchoolOutdoor, LoadSceneMode.Additive));

        StartCoroutine(GetSceneLoadProgress());
    }

    public void LoadBattleSchoolOutdoor()
    {
        LoadingScreenSetActive();

        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.World_SchoolOutdoor));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.Battlefield_SchoolOutdoor, LoadSceneMode.Additive));

        StartCoroutine(GetSceneLoadProgress());
    }

    public void LoadWorldSchoolOutdoor()
    {
        LoadingScreenSetActive();

        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.Battlefield_SchoolOutdoor));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.World_SchoolOutdoor, LoadSceneMode.Additive));

        StartCoroutine(GetSceneLoadProgress());
    }

    public void LoadBattleParkinglot()
    {
        LoadingScreenSetActive();

        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.World_Parkinglot));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.Battlefield_Parkinglot, LoadSceneMode.Additive));

        StartCoroutine(GetSceneLoadProgress());
    }

    public void LoadWorldParkinglot()
    {
        LoadingScreenSetActive();

        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.Battlefield_Parkinglot));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.World_Parkinglot, LoadSceneMode.Additive));

        StartCoroutine(GetSceneLoadProgress());
    }

    public void EnterWorldParkinglot()
    {
        LoadingScreenSetActive();

        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.World_SchoolOutdoor));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.World_Parkinglot, LoadSceneMode.Additive));

        StartCoroutine(GetSceneLoadProgress());
    }

    float totalSceneProgress;
    public IEnumerator GetSceneLoadProgress()
    {
        for(int i = 0; i < scenesLoading.Count; i++)
        {
            while (!scenesLoading[i].isDone)
            {
                totalSceneProgress = 0;

                foreach(AsyncOperation operation in scenesLoading)
                {
                    totalSceneProgress += operation.progress;
                }
                totalSceneProgress = (totalSceneProgress / scenesLoading.Count) * 100;
                progressBar.value = Mathf.RoundToInt(totalSceneProgress);

                yield return null;
            }
        }

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeOutLoadingScreen());
    }

    private void LoadingScreenSetActive()
    {
        loadingScreen.gameObject.SetActive(true);
        CanvasGroup canvasGroup = loadingScreen.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = loadingScreen.gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 1f;
    }
    private IEnumerator FadeOutLoadingScreen()
    {
        CanvasGroup canvasGroup = loadingScreen.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = loadingScreen.gameObject.AddComponent<CanvasGroup>();
        }

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
