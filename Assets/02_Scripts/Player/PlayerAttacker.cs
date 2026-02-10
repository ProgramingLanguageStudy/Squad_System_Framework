using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격 관련 로직 담당. HitboxController(히트 구간 켜기/끄기) + 감지 시 데미지 적용.
/// 입력은 Player.RequestAttack()으로 처리.
/// 공격 시작/끝: 애니 이벤트로 호출. 히트 구간만 Animation_BeginHitWindow / Animation_EndHitWindow 로 연결.
/// 한 번의 공격(OnAttackStarted~OnAttackEnded)당 같은 대상에는 한 번만 데미지 적용.
/// </summary>
public class PlayerAttacker : MonoBehaviour
{
    [SerializeField] [Tooltip("히트박스. 켜기/끄기 + 감지 이벤트 구독")]
    private HitboxController _hitboxController;

    private PlayerModel _ownerModel;
    private PlayerStateMachine _stateMachine;
    private readonly HashSet<IDamageable> _hitThisAttack = new HashSet<IDamageable>();

    public void Initialize(PlayerStateMachine stateMachine, PlayerModel ownerModel)
    {
        _stateMachine = stateMachine;
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

    /// <summary>공격 시작. 애니 이벤트로 호출. _hitThisAttack 초기화 + 이펙트·사운드용.</summary>
    public void OnAttackStarted()
    {
        _hitThisAttack.Clear();
        // TODO: 무기 잔상, 이펙트, 사운드 재생
    }

    /// <summary>[애니 이벤트] 히트 판정 구간 시작 (타격 프레임). 이 시점에만 히트박스 켬.</summary>
    public void Animation_BeginHitWindow()
    {
        _hitboxController?.EnableHit();
    }

    /// <summary>[애니 이벤트] 히트 판정 구간 끝.</summary>
    public void Animation_EndHitWindow()
    {
        _hitboxController?.DisableHit();
    }

    /// <summary>공격 종료. 애니 이벤트로 호출. 히트박스 끄고 Idle 전환.</summary>
    public void OnAttackEnded()
    {
        // TODO: 이펙트 정리 등
    }
}
