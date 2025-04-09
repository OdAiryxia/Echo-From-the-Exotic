using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    [SerializeField, TextArea(3, 10)] private string[] _dialogue;

    public string InteractablePrompt => _prompt;
    public bool Interact(Interactor interactor)
    {
        int index = Random.Range(0, _dialogue.Length);
        InteractorDialog.instance.StartTyping(_dialogue[index]);
        return true;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1.0f, $"Interactable Object: {_prompt}");
    }
#endif
}
