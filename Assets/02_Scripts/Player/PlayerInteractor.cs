using System;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("----- Detection Settings -----")]
    [SerializeField] private Transform _detectPoint;
    [SerializeField] private float _distance = 3f;
    [SerializeField] private float _radius = 0.5f; // 구체의 반지름
    [SerializeField] private LayerMask _targetLayerMask;

    public IInteractable CurrentTarget { get; private set; }

    private IInteractable _lastTarget;
    private Player _player;
    private InputHandler _input;

    // NonAlloc을 위한 결과 저장 배열 (보통 1개만 필요하면 크기를 1로 잡습니다)
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
        ExecuteInteraction();
    }

    private void Detect()
    {
        Vector3 origin = _detectPoint != null ? _detectPoint.position : transform.position + Vector3.up;
        Vector3 direction = _detectPoint != null ? _detectPoint.forward : transform.forward;

        IInteractable found = null;

        // SphereCastNonAlloc 실행 (감지된 개수를 반환)
        int numHit = Physics.SphereCastNonAlloc(origin, _radius, direction, _results, _distance, _targetLayerMask);

        if (numHit > 0)
        {
            found = _results[0].collider.GetComponentInParent<IInteractable>();
        }
        else
        {
            // 감지된 것이 없으면 명확히 null 처리
            found = null;
        }

        if (found != _lastTarget)
        {
            _lastTarget = found;
            CurrentTarget = found;
            OnTargetChanged?.Invoke(found);
        }
    }

    private void ExecuteInteraction()
    {
        // 입력 신호가 들어왔다면
        if (_input != null && _input.InteractTriggered)
        {
            // 1. 타겟이 있다면 상호작용 실행
            if (CurrentTarget != null)
            {
                CurrentTarget.Interact(_player);
            }

            // 2. 중요: 타겟이 있든 없든, 'E'를 눌렀다면 그 신호를 즉시 소모함
            // 이렇게 하면 허공에서 누른 'E'가 나중에 타겟을 만났을 때까지 살아남지 못합니다.
            _input.ResetInteractTrigger();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 origin = _detectPoint != null ? _detectPoint.position : transform.position + Vector3.up;
        Vector3 direction = _detectPoint != null ? _detectPoint.forward : transform.forward;

        // SphereCast의 궤적 시각화
        Gizmos.DrawLine(origin, origin + direction * _distance);
        Gizmos.DrawWireSphere(origin + direction * _distance, _radius);
    }
}