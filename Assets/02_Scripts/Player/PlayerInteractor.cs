using System;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("----- Detection Settings -----")]
    [SerializeField] private Transform _detectPoint;
    [SerializeField] private float _distance = 3f;
    [SerializeField] private float _radius = 0.5f;
    [SerializeField] private LayerMask _targetLayerMask;

    public IInteractable CurrentTarget { get; private set; }

    private IInteractable _lastTarget;
    private Player _player;
    private InputHandler _input;
    private RaycastHit[] _results = new RaycastHit[1];

    public event Action<IInteractable> OnTargetChanged;

    public void Initialize(Player player, InputHandler input)
    {
        _player = player;
        _input = input;
    }

    private void Update()
    {
        Detect();
    }

    private void Detect()
    {
        Vector3 origin = _detectPoint != null ? _detectPoint.position : transform.position + Vector3.up;
        Vector3 direction = _detectPoint != null ? _detectPoint.forward : transform.forward;

        IInteractable found = null;
        int numHit = Physics.SphereCastNonAlloc(origin, _radius, direction, _results, _distance, _targetLayerMask);

        if (numHit > 0)
        {
            if (_results[0].collider.transform.TryGetComponent<IInteractable>(out var interactable))
            {
                found = interactable;
            }
        }

        if (found != _lastTarget)
        {
            _lastTarget = found;
            CurrentTarget = found;
            OnTargetChanged?.Invoke(found);
        }
    }

    // [중요] PlayScene에서 이 함수를 호출하게 됩니다.
    public void TryInteract()
    {
        if (CurrentTarget != null)
        {
            Debug.Log($"{CurrentTarget}와 상호작용 시도");
            CurrentTarget.Interact(_player);
        }
        else
        {
            Debug.Log("상호작용 대상이 없습니다.");
        }
    }
}