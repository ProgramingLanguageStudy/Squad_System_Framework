using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy 공격. HitboxController 히트박스 켜기/끄기 + 감지 시 데미지 적용.
/// 공격 시작/끝: 상태(Enter·타이머)에서 호출. 히트 구간만 애니 이벤트: Animation_BeginHitWindow / Animation_EndHitWindow.
/// </summary>
public class EnemyAttacker : MonoBehaviour
{
    [SerializeField] [Tooltip("히트박스. 켜기/끄기 + 감지 이벤트 구독")]
    private HitboxController _hitboxController;

    private EnemyModel _ownerModel;
    private readonly HashSet<IDamageable> _hitThisAttack = new HashSet<IDamageable>();

    /// <summary>Enemy가 Model 주입 시 호출.</summary>
    public void Initialize(EnemyModel ownerModel)
    {
        _ownerModel = ownerModel;
    }

    private void OnEnable()
    {
        if (_hitboxController != null)
            _hitboxController.OnDamageableDetected += ApplyDamage;
    }

    private void OnDisable()
    {
        if (_hitboxController != null)
            _hitboxController.OnDamageableDetected -= ApplyDamage;
    }

    private void ApplyDamage(IDamageable target)
    {
        if (target == null || _ownerModel == null) return;
        if (ReferenceEquals(target, _ownerModel)) return;
        if (_hitThisAttack.Contains(target)) return;

        _hitThisAttack.Add(target);
        target.TakeDamage(_ownerModel.AttackPower);
    }

    /// <summary>공격 시작. 상태 Enter에서 호출. 이펙트·사운드용. 히트박스는 Animation_BeginHitWindow에서.</summary>
    public void OnAttackStarted()
    {
        // TODO: 이펙트, 사운드
    }

    /// <summary>[애니 이벤트] 히트 판정 구간 시작. 이 시점에만 히트박스 켬.</summary>
    public void Animation_BeginHitWindow()
    {
        _hitboxController?.EnableHit();
    }

    /// <summary>[애니 이벤트] 히트 판정 구간 끝.</summary>
    public void Animation_EndHitWindow()
    {
        _hitboxController?.DisableHit();
    }

    /// <summary>공격 종료. 상태(타이머) 또는 애니 이벤트에서 호출. 히트박스 확실히 끔.</summary>
    public void OnAttackEnded()
    {
        // TODO: 이펙트, 사운드
    }
}
