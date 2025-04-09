using UnityEngine;
using Cinemachine;


#if UNITY_EDITOR
using UnityEditor;
#endif
public class CreateDummyPos : MonoBehaviour
{
    [ContextMenu("Create Story Position Dummy")]
    void CreateDummyStoryPosition()
    {
        int index = 0;

        GameObject root = new GameObject($"StoryPos_{index}");

        GameObject camPos = new GameObject("CamPos");
        camPos.transform.SetParent(root.transform);
        camPos.transform.localPosition = new Vector3(-1, 1.5f, -3);
        camPos.AddComponent<StoryGizmo>().gizmoType = StoryGizmo.GizmoType.Camera;
        camPos.AddComponent<CinemachineFreeLook>();

        GameObject playerPos = new GameObject("PlayerPos");
        playerPos.transform.SetParent(root.transform);
        playerPos.transform.localPosition = new Vector3(0, 0, 0);
        playerPos.AddComponent<StoryGizmo>().gizmoType = StoryGizmo.GizmoType.Player;

        GameObject otherPos = new GameObject("OtherPos");
        otherPos.transform.SetParent(root.transform);
        otherPos.transform.localPosition = new Vector3(1.5f, 0, 1.5f);
        otherPos.AddComponent<StoryGizmo>().gizmoType = StoryGizmo.GizmoType.Other;

        root.transform.position = transform.position + Vector3.forward * (index * 2);

        Debug.Log("定位空物件建立完成，請手動拖曳到 StoryPositions 陣列中。");
    }
}
