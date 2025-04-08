using UnityEngine;

public class SceneTransmitter : MonoBehaviour, IInteractable
{
    [SerializeField] private WorldIndexes scene;
    [SerializeField] private string spawnpointID;
    [SerializeField] private string prompt;

    public string InteractablePrompt => prompt;
    public bool Interact(Interactor interactor)
    {
        GameManager.instance.LoadWorld(scene, spawnpointID);
        return true;
    }

    void OnDrawGizmos()
    {
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1.0f, $"→ {scene} : {spawnpointID}");
    }
}
