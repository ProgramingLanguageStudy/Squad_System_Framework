using UnityEngine;

/// <summary>
/// 공격 관련 로직 담당. Weapon(히트 구간)과 DamageableDetector(감지)를 보유하고, 애니 이벤트를 여기서 받아 위임.
/// 입력은 Player.RequestAttack()으로 처리. 애니메이션 이벤트: EnableAttackHit() → Weapon, EndAttack() → DisableHit + Idle.
/// </summary>
public class PlayerAttacker : MonoBehaviour
{
    [SerializeField] [Tooltip("무기. 히트박스 켜기/끄기 + 감지 이벤트는 Weapon 통해 전달")]
    private Weapon _weapon;

    private PlayerStateMachine _stateMachine;

    public void Initialize(PlayerStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    /// <summary>애니메이션 이벤트: 공격이 적에 닿는 구간 시작 시 호출.</summary>
    public void EnableAttackHit()
    {
        _weapon?.EnableHit();
    }

    /// <summary>애니메이션 이벤트: 공격 히트 구간 끝 또는 수동 비활성 시 호출.</summary>
    public void DisableAttackHit()
    {
        _weapon?.DisableHit();
    }

    /// <summary>공격 애니메이션 끝날 때 호출. 애니메이션 이벤트에서 연결.</summary>
    public void EndAttack()
    {
        DisableAttackHit();
        _stateMachine?.RequestIdle();
    }
}
