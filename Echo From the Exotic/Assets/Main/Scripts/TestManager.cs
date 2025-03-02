using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestManager : MonoBehaviour
{
    [SerializeField] private bool UI;
    [SerializeField] private bool PersistentScene;

    [SerializeField] private SceneIndexes currentWorldScene;

    private bool prologuePlayed = false;
    void Awake()
    {

        prologuePlayed = PlayerPrefs.GetInt("ProloguePlayed", 0) == 1;
        StartCoroutine(Debug());
        if (prologuePlayed == false)
        {
            ProgressManager.instance.StartDialogue(0);
        }
    }

    IEnumerator Debug()
    {
        if (UI)
        {
            if (!SceneManager.GetSceneByBuildIndex((int)SceneIndexes.UI).isLoaded)
            {
                AsyncOperation loadUI = SceneManager.LoadSceneAsync((int)SceneIndexes.UI, LoadSceneMode.Additive);
                while (!loadUI.isDone) // 等待場景完全載入
                {
                    yield return null;
                }
            }
        }

        if (PersistentScene)
        {
            if (!SceneManager.GetSceneByBuildIndex((int)SceneIndexes.PersistentScene).isLoaded)
            {
                AsyncOperation loadPersistent = SceneManager.LoadSceneAsync((int)SceneIndexes.PersistentScene, LoadSceneMode.Additive);
                while (!loadPersistent.isDone) // 等待場景完全載入
                {
                    yield return null;
                }

                GameManager.instance.currentWorldScene = currentWorldScene;
            }
        }
    }
}
