using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestManager : MonoBehaviour
{
    [SerializeField] private bool UI;
    [SerializeField] private bool PersistentScene;
    [SerializeField] private bool reEnterGame;

    [SerializeField] private GameObject player;
    void Awake()
    {
        Debug();
    }

    void Debug()
    {
        if (UI)
        {
            if (!SceneManager.GetSceneByBuildIndex((int)SceneIndexes.UI).isLoaded)
            {
                SceneManager.LoadSceneAsync((int)SceneIndexes.UI, LoadSceneMode.Additive);
            }
        }

        if (PersistentScene)
        {
            // 確保 PersistentScene 沒有載入才 LoadSceneAsync
            if (!SceneManager.GetSceneByBuildIndex((int)SceneIndexes.PersistentScene).isLoaded)
            {
                SceneManager.LoadSceneAsync((int)SceneIndexes.PersistentScene, LoadSceneMode.Additive);
            }

            if (reEnterGame)
            {
                GameManager.instance.reEnterGame = reEnterGame;
            }
        }
    }
}
