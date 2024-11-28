using UnityEngine;

public class InteractionObjectTemplateOngoing : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    [SerializeField, TextArea(3, 10)] private string[] _dialogue;
    private int currentDialogueIndex = 0;

    public string InteractablePrompt => _prompt;
    public bool Interact(Interactor interactor)
    {
        StartCoroutine(TypewriterEffect.instance.ShowText(_dialogue[currentDialogueIndex]));
        if (currentDialogueIndex != _dialogue.Length - 1)
        {
            currentDialogueIndex++;
        }
        return true;
    }
}
