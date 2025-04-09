using UnityEngine;

public class InteractableOngoing : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    [SerializeField, TextArea(3, 10)] private string[] _dialogue;
    private int currentDialogueIndex = 0;

    public string InteractablePrompt => _prompt;
    public bool Interact(Interactor interactor)
    {
        InteractorDialog.instance.StartTyping(_dialogue[currentDialogueIndex]);
        if (currentDialogueIndex != _dialogue.Length - 1)
        {
            currentDialogueIndex++;
        }
        return true;
    }

    void Start()
    {
        this.gameObject.layer = 6;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1.0f, $"Interactable: {_prompt}");
    }
#endif
}
