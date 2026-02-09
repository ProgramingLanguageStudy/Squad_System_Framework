using System;
using UnityEngine;

/// <summary>
/// Trigger에 들어온 IDamageable만 감지해 이벤트로 알림. Collider는 Weapon이 주입하고, 켜기/끄기도 Weapon 담당.
/// Weapon이 이 컴포넌트를 보유하고 Initialize(hitbox)로 Collider를 넣어 줌. 같은 GO에 있어야 OnTriggerEnter 수신 가능.
/// </summary>
public class DamageableDetector : MonoBehaviour
{
    private Collider _collider;

    /// <summary>Trigger에 들어온 대상을 감지했을 때. Weapon을 통해 구독.</summary>
    public event Action<IDamageable> OnDamageableDetected;

    /// <summary>Weapon에서 히트박스 Collider 주입. 같은 GameObject에 Collider 있어야 Trigger 수신됨.</summary>
    public void Initialize(Collider hitbox)
    {
        _collider = hitbox;
        if (_collider != null)
            _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null)
            OnDamageableDetected?.Invoke(damageable);
    }
}
