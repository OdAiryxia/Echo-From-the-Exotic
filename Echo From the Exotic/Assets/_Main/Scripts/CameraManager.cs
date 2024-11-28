using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    //public CinemachineFreeLook[] cameras;

    //public CinemachineFreeLook worldCamera;
    //public CinemachineFreeLook aimCamera;

    //private CinemachineFreeLook startCamera;
    //private CinemachineFreeLook currentCamera;

    private void Start()
    {
        //startCamera = worldCamera;
        //currentCamera = startCamera;

        //for (int i = 0; i < cameras.Length; i++)
        //{
        //    if (cameras[i] == currentCamera)
        //        cameras[i].Priority = 20;
        //    else
        //        cameras[i].Priority = 10;
        //}
    }

    public void Update()
    {
        //if (Input.GetMouseButton(1))
        //{
        //    SwitchCamera(aimCamera);
        //}
        //else
        //{
        //    SwitchCamera(worldCamera);
        //}
    }

    //public void SwitchCamera(CinemachineFreeLook newCam)
    //{
    //    currentCamera = newCam;

    //    currentCamera.Priority = 20;

    //    for (int i = 0; i < cameras.Length; i++)
    //    {
    //        if (cameras[i] != currentCamera)
    //            cameras[i].Priority = 10;
    //    }
    //}
}
