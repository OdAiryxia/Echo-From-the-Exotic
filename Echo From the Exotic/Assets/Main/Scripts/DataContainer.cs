using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DataContainer : MonoBehaviour
{
    public static DataContainer instance;

    public List<GameObject> battlePrefabPlayer;
    public List<GameObject> battlePrefabEnemy;

    public List<string> removalTrigger;
    public List<string> pendingTrigger;

    public bool nextCptAfterBattle;

    public string playerName = "數媒系學生";

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
}
