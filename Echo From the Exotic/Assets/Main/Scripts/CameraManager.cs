using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CinemachineFreeLook thirdPersonCamera;
    public CinemachineVirtualCamera aimCamera;
    public Transform player;  // 角色的 Transform
    public float rotationSpeed = 5f;

    private bool isAiming = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // 按下右鍵進入瞄準模式
        {
            aimCamera.Priority = 20;
            thirdPersonCamera.Priority = 10;
            isAiming = true;
        }
        else if (Input.GetMouseButtonUp(1)) // 放開右鍵回到第三人稱視角
        {
            aimCamera.Priority = 10;
            thirdPersonCamera.Priority = 20;
            isAiming = false;
        }

        if (isAiming)
        {
            RotatePlayerWithCamera();
        }
    }

    void RotatePlayerWithCamera()
    {
        // 讓角色跟隨相機旋轉
        Vector3 cameraForward = aimCamera.transform.forward;
        cameraForward.y = 0; // 防止角色上下旋轉
        Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
        player.rotation = Quaternion.Slerp(player.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
