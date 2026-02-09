using System;
using UnityEngine;

/// <summary>
/// 무기 = 히트박스(Collider) 소유 + DamageableDetector 보유. Collider를 주입식으로 Detector에 넘김.
/// 히트박스는 자식 등 직접 등록. 비어 있으면 로그 후 null 처리.
/// </summary>
public class Weapon : MonoBehaviour
{
    [SerializeField] [Tooltip("이 무기의 히트박스. 자식 오브젝트 등에서 직접 등록")]
    private Collider _hitbox;
    [SerializeField] [Tooltip("감지 담당. 같은 GO에 두고 Initialize로 _hitbox 주입")]
    private DamageableDetector _detector;

    /// <summary>Detector가 IDamageable 감지 시. Player(HitDamageApplier 등)는 Weapon만 구독.</summary>
    public event Action<IDamageable> OnDamageableDetected;

    private void Awake()
    {
        if (_hitbox == null)
        {
            Debug.LogWarning($"[Weapon] {gameObject.name}: 히트박스 Collider가 할당되지 않았습니다. 인스펙터에서 직접 등록해 주세요.");
        }
        else
        {
            _hitbox.isTrigger = true;
            _hitbox.enabled = false;
        }

        if (_detector != null)
        {
            _detector.Initialize(_hitbox);
            _detector.OnDamageableDetected += d => OnDamageableDetected?.Invoke(d);
        }
    }

    /// <summary>애니메이션 이벤트: 히트 활성 구간 시작 시 호출.</summary>
    public void EnableHit()
    {
        if (_hitbox != null) _hitbox.enabled = true;
    }

    /// <summary>애니메이션 이벤트: 히트 활성 구간 끝 시 호출.</summary>
    public void DisableHit()
    {
        if (_hitbox != null) _hitbox.enabled = false;
    }
}
