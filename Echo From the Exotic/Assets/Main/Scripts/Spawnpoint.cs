using Unity.VisualScripting;
using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    public string spawnpointID;

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1f, spawnpointID);
    }
#endif
}
