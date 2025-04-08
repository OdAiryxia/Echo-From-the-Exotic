using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    public string spawnpointID;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1f, spawnpointID);
    }
}
