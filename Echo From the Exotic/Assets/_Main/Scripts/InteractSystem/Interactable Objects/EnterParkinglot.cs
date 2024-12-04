using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterParkinglot : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    [SerializeField] private TypewriterEffect _interactionDialogueUI;

    public string InteractablePrompt => _prompt;
    public bool Interact(Interactor interactor)
    {
        StartCoroutine(_interactionDialogueUI.ShowText("停車場"));

        GameManager.instance.LoadWorldParkinglot();
        return true;
    }
}
