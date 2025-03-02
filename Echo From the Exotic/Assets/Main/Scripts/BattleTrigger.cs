using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleScene
{
    SchoolOutdoor,
    Parkinglot
}

public class BattleTrigger : MonoBehaviour
{
    public List<GameObject> enemiesToSpawn;

    public BattleScene battleScene;
    public void ExecuteAction()
    {
        DataContainer.instance.battlePrefebEnemy.Clear();

        foreach (var prefab in enemiesToSpawn)
        {
            if (prefab != null)
            {
                DataContainer.instance.battlePrefebEnemy.Add(prefab);
            }
        }

        if (battleScene == BattleScene.SchoolOutdoor)
        {
            GameManager.instance.LoadBattleScene(SceneIndexes.Battlefield_SchoolOutdoor);
        }
        else if (battleScene == BattleScene.Parkinglot)
        {
            GameManager.instance.LoadBattleScene(SceneIndexes.Battlefield_Parkinglot);
        }
    }
}
