using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Transform mainCam;

    void Awake()
    {
        mainCam = GameObject.Find("Main Camera").GetComponent<Transform>();
    }

    private void LateUpdate()
    {
        transform.LookAt(mainCam);
        transform.RotateAround(transform.position, transform.up, 180f);
    }
}
