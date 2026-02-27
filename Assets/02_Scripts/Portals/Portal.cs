using DG.Tweening;
using System;
using UnityEngine;

[RequireComponent(typeof(PortalEffect))]
/// <summary>
/// 순간이동 포탈/돌. 상호작용 시 자기 이벤트만 발행(IInteractReceiver, 자기자신). Controller가 구독해 목록 UI → 선택 시 여기로 워프. IInteractable 구현.
/// </summary>
public class Portal : MonoBehaviour, IInteractable
{
    [SerializeField] [Tooltip("포탈에 데이터 연결")]
    private PortalData _data;

    [SerializeField] [Tooltip("도착 시 포탈 앞으로 떨어질 거리. 기본 2. 특수한 경우만 오버라이드용")]
    private float _arrivalDistance = 2f;

    [SerializeField] [Tooltip("선택) 이 포탈만 도착 위치를 따로 쓰고 싶을 때만 할당. 비면 포탈 앞 _arrivalDistance만큼")]
    private Transform _arrivalPointOverride;

    [Tooltip("연출용 스크립트 연결(에셋에서 제공됨)")]
    [SerializeField] PortalEffect _effect;
    [SerializeField] PortalDetector _detector;

    [Header("해금여부 확인용")]
    [SerializeField] private bool _isUnlocked = false;

    /// <summary> 상호작용 시 발행. (IInteractReceiver, 이 포탈). Controller가 구독해 목록/UI 처리. </summary>
    public event Action<IInteractReceiver, Portal> OnInteracted;

    /// <summary> 표시용 데이터 (이름·설명·아이콘). UI에서 사용. </summary>
    public PortalData Data => _data;

    /// <summary> 이 포탈을 선택했을 때 플레이어가 도착할 위치. UI에서 player.Teleport(portal.ArrivalPosition) 호출용. </summary>
    public Vector3 ArrivalPosition => _arrivalPointOverride != null
        ? _arrivalPointOverride.position
        : transform.position + transform.forward * _arrivalDistance;

    public string GetInteractText() => "순간이동";

    private void Awake()
    {
        if (_effect == null)
        {
            _effect = GetComponent<PortalEffect>();
        }
        if (_detector == null)
        {
            _detector = GetComponentInChildren<PortalDetector>();
        }

        if (_detector != null)
        {
            _detector.OnDetectPlayer -= TogglePortalEffect; // 중복 방어
            _detector.OnDetectPlayer += TogglePortalEffect;
        }
    }

    public void Interact(IInteractReceiver receiver)
    {
        if (receiver == null) return;
        OnInteracted?.Invoke(receiver, this);
    }

    public void Initialize(bool isUnlocked)
    {
        _isUnlocked = isUnlocked;
    }

    private void TogglePortalEffect(bool isActive)
    {
        _effect.TogglePortal(isActive);
        Debug.Log("이펙트" + isActive);
    }
    

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // 도착 지점을 하늘색 구체와 선으로 표시해줍니다.
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(ArrivalPosition, 0.3f);
        Gizmos.DrawLine(transform.position, ArrivalPosition);

        // 정면 방향 표시
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 1f);
    }
#endif
}
