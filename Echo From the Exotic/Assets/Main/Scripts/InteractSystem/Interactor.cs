using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionPointRadius = 0.5f;
    [SerializeField] private LayerMask _interactableMask;

    private readonly Collider[] _colliders = new Collider[3];
    private List<Collider> _currentInteractables = new List<Collider>();
    [SerializeField] private int _numfound;

    private IInteractable _interactable;

    void Update()
    {
        _numfound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);
        List<Collider> newColliders = new List<Collider>();

        for (int i = 0; i < _numfound; i++)
        {
            if (_colliders[i] == null) continue;
            newColliders.Add(_colliders[i]);
        }

        // 移除已離開範圍的物件
        for (int i = _currentInteractables.Count - 1; i >= 0; i--)
        {
            if (!newColliders.Contains(_currentInteractables[i])) // 如果舊物件不在新清單內
            {
                if (_interactable != null && _interactable == _currentInteractables[i].GetComponent<IInteractable>())
                {
                    _interactable = null; // 如果當前的 _interactable 剛好是要移除的，則清除
                }
                _currentInteractables.RemoveAt(i); // 移除已離開範圍的物件
            }
        }

        // 更新當前範圍內的可互動物件清單
        _currentInteractables = newColliders;

        // 設置新的最近互動目標
        if (_interactable == null && _currentInteractables.Count > 0)
        {
            _interactable = _currentInteractables[0].GetComponent<IInteractable>();
        }

        // 顯示 UI 提示
        if (_interactable != null)
        {
            if (InteractionPromptUI.instance != null && !InteractionPromptUI.instance.IsDisplayed)
            {
                InteractionPromptUI.instance.SetUp(_interactable.InteractablePrompt);
            }

            // 監聽按鍵輸入
            if (Keyboard.current != null && Input.GetKeyDown(KeyCode.E))
            {
                _interactable.Interact(this);
            }
        }
        else
        {
            // 當沒有可互動物件時，關閉 UI 提示
            if (InteractionPromptUI.instance != null && InteractionPromptUI.instance.IsDisplayed)
            {
                InteractionPromptUI.instance.Close();
            }
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
    }
}
