using System;
using UnityEngine;

/// <summary>
/// 순간이동 포탈/돌. 상호작용 시 자기 이벤트만 발행(IInteractReceiver, 자기자신). Controller가 구독해 목록 UI → 선택 시 여기로 워프. IInteractable 구현.
/// </summary>
public class Portal : MonoBehaviour, IInteractable
{
    [SerializeField] [Tooltip("표시 이름·아이콘 등 (선택). 비면 오브젝트 이름 사용")]
    private PortalData _data;

    [SerializeField] [Tooltip("도착 시 포탈 앞으로 떨어질 거리. 기본 2. 특수한 경우만 오버라이드용")]
    private float _arrivalDistance = 2f;

    [SerializeField] [Tooltip("선택) 이 포탈만 도착 위치를 따로 쓰고 싶을 때만 할당. 비면 포탈 앞 _arrivalDistance만큼")]
    private Transform _arrivalPointOverride;

    /// <summary> 상호작용 시 발행. (IInteractReceiver, 이 포탈). Controller가 구독해 목록/UI 처리. </summary>
    public event Action<IInteractReceiver, Portal> OnInteracted;

    /// <summary> 순간이동 목록 등에 쓸 이름. Data 없으면 오브젝트 이름. </summary>
    public string DisplayName => _data != null && !string.IsNullOrEmpty(_data.displayName)
        ? _data.displayName
        : name;

    /// <summary> 표시용 데이터 (이름·설명·아이콘). UI에서 사용. </summary>
    public PortalData Data => _data;

    /// <summary> 이 포탈을 선택했을 때 플레이어가 도착할 위치. UI에서 player.Teleport(portal.ArrivalPosition) 호출용. </summary>
    public Vector3 ArrivalPosition => _arrivalPointOverride != null
        ? _arrivalPointOverride.position
        : transform.position + transform.forward * _arrivalDistance;

    public string GetInteractText() => "순간이동";

    public void Interact(IInteractReceiver receiver)
    {
        if (receiver == null) return;
        OnInteracted?.Invoke(receiver, this);
    }
}
