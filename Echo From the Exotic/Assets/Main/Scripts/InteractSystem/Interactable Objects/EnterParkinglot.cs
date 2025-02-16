using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterParkinglot : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;

    public string InteractablePrompt => _prompt;
    public bool Interact(Interactor interactor)
    {
        InteractorDialog.instance.StartTyping("停車場");

        GameManager.instance.EnterWorldParkinglot();
        return true;
    }
}
