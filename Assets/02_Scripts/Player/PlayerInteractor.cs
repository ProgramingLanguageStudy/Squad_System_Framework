using System;
using UnityEngine;

/// <summary>
/// 플레이어 주변 상호작용 대상 감지. 반경 내 IInteractable 중 가장 가까운 것을 CurrentTarget으로.
/// (선택) 정면 각도 제한으로 뒤쪽 대상 제외 가능.
/// </summary>
public class PlayerInteractor : MonoBehaviour
{
    [Header("----- Detection Settings -----")]
    [SerializeField] [Tooltip("감지 기준 위치. 비면 플레이어 위치 + Vector3.up")]
    private Transform _detectPoint;
    [SerializeField] [Tooltip("감지 반경. 이 거리 안의 상호작용 가능 오브젝트 후보")]
    private float _radius = 3f;
    [SerializeField] [Tooltip("정면 기준 각도 제한(도). 360이면 전방위, 120이면 앞 120°만")]
    private float _maxAngle = 180f;
    [SerializeField] private LayerMask _targetLayerMask;
    [SerializeField] [Tooltip("감지 결과 캐시용. 크기 부족 시 자동 확대")]
    private int _overlapBufferSize = 16;

    public IInteractable CurrentTarget { get; private set; }

    private IInteractable _lastTarget;
    private Player _player;
    private Collider[] _overlapBuffer;

    public event Action<IInteractable> OnTargetChanged;

    public void Initialize(Player player)
    {
        _player = player;
        _overlapBuffer = new Collider[_overlapBufferSize];
    }

    private void Update()
    {
        Detect();
    }

    private void Detect()
    {
        Vector3 origin = _detectPoint != null ? _detectPoint.position : transform.position + Vector3.up;
        Vector3 forward = _detectPoint != null ? _detectPoint.forward : transform.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.01f) forward = transform.forward;
        forward.Normalize();

        IInteractable found = null;
        float closestSqr = _radius * _radius;

        int count = Physics.OverlapSphereNonAlloc(origin, _radius, _overlapBuffer, _targetLayerMask);
        for (int i = 0; i < count; i++)
        {
            Collider c = _overlapBuffer[i];
            if (c == null) continue;
            if (!c.transform.TryGetComponent<IInteractable>(out var interactable)) continue;

            Vector3 toTarget = (c.ClosestPoint(origin) - origin);
            toTarget.y = 0f;
            float sqrDist = toTarget.sqrMagnitude;

            if (_maxAngle < 360f && toTarget.sqrMagnitude >= 0.001f)
            {
                toTarget.Normalize();
                if (Vector3.Angle(forward, toTarget) > _maxAngle * 0.5f) continue;
            }

            if (sqrDist >= closestSqr) continue;
            closestSqr = sqrDist;
            found = interactable;
        }

        if (found != _lastTarget)
        {
            _lastTarget = found;
            CurrentTarget = found;
            OnTargetChanged?.Invoke(found);
            GameEvents.OnInteractTargetChanged?.Invoke(found);
        }
    }

    /// <summary>PlayScene에서 E 입력 시 호출됩니다.</summary>
    public void TryInteract()
    {
        if (CurrentTarget != null)
            CurrentTarget.Interact(_player);
    }
}
