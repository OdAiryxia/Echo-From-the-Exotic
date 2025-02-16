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

        // �����w���}�d�򪺪���
        for (int i = _currentInteractables.Count - 1; i >= 0; i--)
        {
            if (!newColliders.Contains(_currentInteractables[i])) // �p�G�ª��󤣦b�s�M�椺
            {
                if (_interactable != null && _interactable == _currentInteractables[i].GetComponent<IInteractable>())
                {
                    _interactable = null; // �p�G��e�� _interactable ��n�O�n�������A�h�M��
                }
                _currentInteractables.RemoveAt(i); // �����w���}�d�򪺪���
            }
        }

        // ��s��e�d�򤺪��i���ʪ���M��
        _currentInteractables = newColliders;

        // �]�m�s���̪񤬰ʥؼ�
        if (_interactable == null && _currentInteractables.Count > 0)
        {
            _interactable = _currentInteractables[0].GetComponent<IInteractable>();
        }

        // ��� UI ����
        if (_interactable != null)
        {
            if (InteractionPromptUI.instance != null && !InteractionPromptUI.instance.IsDisplayed)
            {
                InteractionPromptUI.instance.SetUp(_interactable.InteractablePrompt);
            }

            // ��ť�����J
            if (Keyboard.current != null && Input.GetKeyDown(KeyCode.E))
            {
                _interactable.Interact(this);
            }
        }
        else
        {
            // ��S���i���ʪ���ɡA���� UI ����
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
