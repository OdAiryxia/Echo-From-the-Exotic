using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EnemyRobot : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    [SerializeField] private TypewriterEffect _interactionDialogueUI;

    public List<GameObject> enemiesToSpawn;

    public string InteractablePrompt => _prompt;
    public bool Interact(Interactor interactor)
    {
        StartCoroutine(_interactionDialogueUI.ShowText("機器人"));

        DataContainer.instance.battlePrefebEnemy.Clear();

        foreach (var prefab in enemiesToSpawn)
        {
            if (prefab != null)
            {
                DataContainer.instance.battlePrefebEnemy.Add(prefab);
            }
        }

        GameManager.instance.LoadBattleSchoolOutdoor();
        return true;
    }
}
