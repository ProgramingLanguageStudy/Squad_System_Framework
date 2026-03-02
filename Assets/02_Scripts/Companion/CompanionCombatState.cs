using UnityEngine;

public class CompanionCombatState : CompanionStateBase
{
    private Enemy _currentTarget;
    private float _targetUpdateTimer;

    public CompanionCombatState(CompanionStateMachine machine) : base(machine) { }

    public override void Enter() => _targetUpdateTimer = 0f;

    public override void Update()
    {
        var combat = Machine.CombatController;
        if (combat == null || !combat.IsInCombat) return;

        _targetUpdateTimer += Time.deltaTime;
        if (_targetUpdateTimer >= Machine.TargetUpdateInterval || _currentTarget == null)
        {
            _targetUpdateTimer = 0f;
            _currentTarget = combat.GetNearestEnemy(Machine.transform.position);
        }

        if (_currentTarget == null || _currentTarget.Model.IsDead) return;

        float dist = Vector3.Distance(Machine.transform.position, _currentTarget.transform.position);
        Machine.Character?.SetCombatTarget(_currentTarget.transform, Machine.AttackRange);

        // 사거리 내 진입 시 공격 상태로 전환 요청
        if (dist <= Machine.AttackRange)
        {
            Machine.RequestAttack();
        }
    }

    public override void Exit() => _currentTarget = null;
}