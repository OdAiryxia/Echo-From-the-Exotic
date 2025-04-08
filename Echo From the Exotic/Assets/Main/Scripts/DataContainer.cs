using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataContainer : MonoBehaviour
{
    public static DataContainer instance;

    public List<GameObject> battlePrefabPlayer;
    public List<GameObject> battlePrefabEnemy;

    public List<string> removalTrigger;
    public List<string> pendingTrigger;

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
