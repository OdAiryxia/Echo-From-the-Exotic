using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCamera : MonoBehaviour
{

    public CinemachineFreeLook vCam;
    private CinemachineFreeLook.Orbit[] originalOrbits;

    [Range(0.5f, 2f)]
    public float zoomPercent = 1f;
    private float scroll;

    void Start()
    {
        originalOrbits = new CinemachineFreeLook.Orbit[vCam.m_Orbits.Length];
        for (int i = 0; i < vCam.m_Orbits.Length; i++)
        {
            originalOrbits[i].m_Height = vCam.m_Orbits[i].m_Height;
            originalOrbits[i].m_Radius = vCam.m_Orbits[i].m_Radius;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < vCam.m_Orbits.Length; i++)
        {
            vCam.m_Orbits[i].m_Height = originalOrbits[i].m_Height * zoomPercent;
            vCam.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius * zoomPercent;
        }
        scroll = Input.mouseScrollDelta.y;
        if (zoomPercent >= 0.5f && zoomPercent <= 2f)
        {
            zoomPercent += -scroll * 0.05f;
        }
        if (zoomPercent < 0.5f)
        {
            zoomPercent = 0.5f;
        }

        if (zoomPercent > 2f)
        {
            zoomPercent = 2f;
        }
    }
}
