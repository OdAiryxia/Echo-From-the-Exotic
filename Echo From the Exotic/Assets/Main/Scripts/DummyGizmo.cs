using UnityEngine;

[ExecuteInEditMode]
public class StoryGizmo : MonoBehaviour
{
    public enum GizmoType { Camera, Player, Other }
    public GizmoType gizmoType = GizmoType.Player;

    public float arrowLength = 1f;

    void OnDrawGizmos()
    {
        switch (gizmoType)
        {
            case GizmoType.Camera:
                Gizmos.color = Color.cyan;
                break;
            case GizmoType.Player:
                Gizmos.color = Color.green;
                break;
            case GizmoType.Other:
                Gizmos.color = Color.yellow;
                break;
        }

        Gizmos.DrawSphere(transform.position, 0.1f);

        Vector3 from = transform.position;
        Vector3 to = transform.position + transform.forward * arrowLength;
        Gizmos.DrawLine(from, to);

        Vector3 right = Quaternion.LookRotation(transform.forward) * Quaternion.Euler(0, 150, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(transform.forward) * Quaternion.Euler(0, -150, 0) * Vector3.forward;
        Gizmos.DrawLine(to, to + right * 0.2f);
        Gizmos.DrawLine(to, to + left * 0.2f);
    }
}