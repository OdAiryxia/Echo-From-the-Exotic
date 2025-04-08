using UnityEngine;

public interface IInteractable
{
    public string InteractablePrompt { get; }
    public bool Interact(Interactor interactor);
}
