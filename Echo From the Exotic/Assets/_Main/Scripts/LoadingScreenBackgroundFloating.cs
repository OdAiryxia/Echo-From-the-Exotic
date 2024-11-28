using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenBackgroundFloating : MonoBehaviour
{
    [SerializeField] private float floatRange = 500f;
    [SerializeField] private float floatSpeed = 1f;

    private Vector3 originalPosition;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
        rectTransform.anchoredPosition = new Vector2(originalPosition.x, originalPosition.y - offset);
    }
}
