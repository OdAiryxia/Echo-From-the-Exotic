using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EnemyRobot : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;

    public string InteractablePrompt => _prompt;
    public bool Interact(Interactor interactor)
    {
        InteractorDialog.instance.StartTyping("機器人");
        return true;
    }
}
