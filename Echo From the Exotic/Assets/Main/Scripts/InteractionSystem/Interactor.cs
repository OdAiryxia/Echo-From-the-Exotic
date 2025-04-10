using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionPointRadius = 0.5f;
    [SerializeField] private LayerMask _interactableMask;

    private readonly Collider[] _colliders = new Collider[3];
    [SerializeField] private int _numfound;

    private IInteractable _interactable;

    //void Update()
    //{
    //    _numfound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);

    //    if (_numfound > 0)
    //    {
    //        _interactable = _colliders[0].GetComponent<IInteractable>();

    //        if (_interactable != null)
    //        {
    //            if (!InteractionPromptUI.instance.IsDisplayed) InteractionPromptUI.instance.SetUp(_interactable.InteractablePrompt);

    //            if (Input.GetKeyDown(KeyCode.E)) _interactable.Interact(this);
    //        }
    //    }
    //    else
    //    {
    //        if (_interactable != null) _interactable = null;
    //        if (InteractionPromptUI.instance.IsDisplayed) InteractionPromptUI.instance.Close();
    //    }
    //}
    void Update()
    {
        _numfound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);

        IInteractable closestInteractable = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < _numfound; i++)
        {
            IInteractable interactable = _colliders[i].GetComponent<IInteractable>();
            if (interactable != null)
            {
                float dist = Vector3.Distance(_interactionPoint.position, _colliders[i].transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestInteractable = interactable;
                }
            }
        }

        if (closestInteractable != null)
        {
            _interactable = closestInteractable;

            // 更新提示 UI（只在內容不同或未顯示時執行）
            if (!InteractionPromptUI.instance.IsDisplayed ||
                InteractionPromptUI.instance.CurrentPrompt != _interactable.InteractablePrompt)
            {
                InteractionPromptUI.instance.SetUp(_interactable.InteractablePrompt);
            }

            // 按下互動鍵
            if (Input.GetKeyDown(KeyCode.E))
            {
                _interactable.Interact(this);
            }
        }
        else
        {
            // 沒有任何互動物件
            if (_interactable != null)
                _interactable = null;

            if (InteractionPromptUI.instance.IsDisplayed)
                InteractionPromptUI.instance.Close();
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
    }
}
