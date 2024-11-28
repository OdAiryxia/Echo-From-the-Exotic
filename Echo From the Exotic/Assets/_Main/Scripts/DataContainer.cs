using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataContainer : MonoBehaviour
{
    public static DataContainer instance;
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

    [Header("戰鬥設置")]
    public List<GameObject> battlePrefebPlayer;
    public List<GameObject> battlePrefebEnemy;
}
