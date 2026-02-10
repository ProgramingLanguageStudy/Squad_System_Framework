using System;
using UnityEngine;

/// <summary>
/// Trigger에 들어온 IDamageable만 감지해 이벤트로 알림.
/// HitboxController와 같은 GO에 두고, 히트박스 Collider도 같은 GO(또는 자식)에 두면 OnTriggerEnter 수신됨. Collider 켜기/끄기는 HitboxController 담당.
/// </summary>
public class DamageableDetector : MonoBehaviour
{
    /// <summary>Trigger에 들어온 대상을 감지했을 때. HitboxController를 통해 구독.</summary>
    public event Action<IDamageable> OnDamageableDetected;

    private void OnTriggerEnter(Collider other)
    {
        var damageable = other.GetComponentInParent<IDamageable>();
        if (damageable != null)
            OnDamageableDetected?.Invoke(damageable);
    }
}
