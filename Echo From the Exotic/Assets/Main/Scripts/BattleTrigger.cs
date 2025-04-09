using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleTrigger : MonoBehaviour
{
    [SerializeField] private string triggerID;

    [SerializeField] private List<GameObject> enemiesToSpawn;
    [SerializeField] private BattlefieldIndexes battlefieldIndex;
    [SerializeField] private UnityEvent events;
    [SerializeField] private ModalWindowTemplate modalWindowTemplate;

    void Start()
    {
        if (DataContainer.instance.removalTrigger.Contains(triggerID))
        {
            Destroy(this.gameObject);
        }
    }

    public void ExecuteBattle()
    {
        DataContainer.instance.pendingTrigger.Clear();
        DataContainer.instance.pendingTrigger.Add(triggerID);

        DataContainer.instance.battlePrefabEnemy = enemiesToSpawn;
        GameManager.instance.LoadBattle(battlefieldIndex);
    }
}
