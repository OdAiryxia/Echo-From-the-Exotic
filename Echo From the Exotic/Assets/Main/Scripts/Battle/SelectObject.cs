using UnityEngine;

public class SelectObject : MonoBehaviour
{
    public GameObject selectedUnit;

    [SerializeField] private LayerMask layerMaskToSelect;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            SelectTarget();
    }

    void SelectTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.MaxValue, layerMaskToSelect))
            selectedUnit = hit.transform.gameObject;
    }
}
