using System;
using UnityEngine;

/// <summary>
/// 히트박스 전담. Collider 소유 + DamageableDetector 보유. 같은 GO에 두면 Detector가 Trigger 자동 수신.
/// 플레이어 무기/늑대 발 등 공격 구간에 붙여서 EnableHit/DisableHit으로 구간만 켜고 끔.
/// </summary>
public class HitboxController : MonoBehaviour
{
    [SerializeField] [Tooltip("히트박스 Collider. 자식 등에서 직접 등록")]
    private Collider _hitbox;
    [SerializeField] [Tooltip("감지 담당. 같은 GO에 두면 됨")]
    private DamageableDetector _detector;

    /// <summary>Detector가 IDamageable 감지 시. PlayerAttacker 등이 구독해 데미지 적용.</summary>
    public event Action<IDamageable> OnDamageableDetected;

    private void Awake()
    {
        if (_hitbox == null)
        {
            Debug.LogWarning($"[HitboxController] {gameObject.name}: 히트박스 Collider가 할당되지 않았습니다. 인스펙터에서 직접 등록해 주세요.");
        }
        else
        {
            _hitbox.isTrigger = true;
            _hitbox.enabled = false;
        }

        if (_detector != null)
            _detector.OnDamageableDetected += d => OnDamageableDetected?.Invoke(d);
    }

    /// <summary>히트 활성 구간 시작 시 호출 (애니 이벤트 등).</summary>
    public void EnableHit()
    {
        if (_hitbox != null) _hitbox.enabled = true;
    }

    /// <summary>히트 활성 구간 끝 시 호출.</summary>
    public void DisableHit()
    {
        if (_hitbox != null) _hitbox.enabled = false;
    }
}
