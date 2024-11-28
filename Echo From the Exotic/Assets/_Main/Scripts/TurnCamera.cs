using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class TurnCamera : MonoBehaviour
{
    public CinemachineFreeLook vCamera;
    public CharacterMovement characterMovement;
    public ZoomCamera zoomCamera;

    public float turnDegree;
    public float turnDuration;

    private bool isRotating = false;
    private float originalMinValue;
    private float originalMaxValue;
    private float targetMinValue;
    private float targetMaxValue;
    private float elapsedTime = 0f;

    public GameObject otherCamTrigger;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isRotating)
        {
            isRotating = true;
            elapsedTime = 0f;
            originalMinValue = vCamera.m_XAxis.m_MinValue;
            originalMaxValue = vCamera.m_XAxis.m_MaxValue;
            targetMinValue = originalMinValue + turnDegree;
            targetMaxValue = originalMaxValue + turnDegree;

            StartCoroutine(RotatingCamera());
        }
    }

    void Update()
    {
        if (isRotating)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / turnDuration);
            vCamera.m_XAxis.m_MinValue = Mathf.Lerp(originalMinValue, targetMinValue, t);
            vCamera.m_XAxis.m_MaxValue = Mathf.Lerp(originalMaxValue, targetMaxValue, t);

            float zoom = zoomCamera.zoomPercent;
            zoomCamera.zoomPercent = Mathf.Lerp(zoom, 2f, t);

            if (elapsedTime >= turnDuration)
            {
                isRotating = false;
            }
        }
    }

    IEnumerator RotatingCamera()
    {
        characterMovement.enabled = false;
        otherCamTrigger.SetActive(true);
        yield return new WaitForSeconds(2);
        characterMovement.enabled = true;
        gameObject.SetActive(false);
    }
}

