using UnityEngine;

[ExecuteInEditMode]
public class StoryGizmo : MonoBehaviour
{
    public enum GizmoType { Camera, Player, Other }
    public GizmoType gizmoType = GizmoType.Player;

    private float arrowLength = 1f;
    private float playerCapsuleHeight = 6f;
    private float playerCapsuleRadius = 0.7f;

    void OnDrawGizmos()
    {
        switch (gizmoType)
        {
            case GizmoType.Camera:
                Gizmos.color = Color.cyan;
                break;
            case GizmoType.Player:
                Gizmos.color = Color.green;
                DrawCapsuleGizmo(transform.position, playerCapsuleRadius, playerCapsuleHeight);
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

    private void DrawCapsuleGizmo(Vector3 position, float radius, float height)
    {
        // 計算膠囊體端點
        Vector3 topCenter = position + Vector3.up * (height * 0.5f - radius);
        Vector3 bottomCenter = position + Vector3.down * (height * 0.5f - radius);

        // 保存原有顏色
        Color originalColor = Gizmos.color;
        // 使用半透明顏色
        Gizmos.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.3f);

        // 繪製膠囊體的球形端部
        Gizmos.DrawWireSphere(topCenter, radius);
        Gizmos.DrawWireSphere(bottomCenter, radius);

        // 繪製連接端部的線條
        Gizmos.DrawLine(topCenter + Vector3.forward * radius, bottomCenter + Vector3.forward * radius);
        Gizmos.DrawLine(topCenter + Vector3.back * radius, bottomCenter + Vector3.back * radius);
        Gizmos.DrawLine(topCenter + Vector3.right * radius, bottomCenter + Vector3.right * radius);
        Gizmos.DrawLine(topCenter + Vector3.left * radius, bottomCenter + Vector3.left * radius);

        // 恢復原有顏色
        Gizmos.color = originalColor;
    }
}