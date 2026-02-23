using UnityEngine;

/// <summary>
/// 전투 상태. 가장 가까운 적 추적, 공격 범위 내면 RequestAttack.
/// </summary>
public class CompanionCombatState : CompanionStateBase
{
    private Enemy _currentTarget;
    private float _targetUpdateTimer;

    public CompanionCombatState(CompanionStateMachine machine) : base(machine) { }

    public override void Enter()
    {
        _currentTarget = null;
        _targetUpdateTimer = 0f;
    }

    public override void Update()
    {
        var character = Machine.Character;
        var combat = Machine.CombatController;
        var player = GameServices.Player?.GetPlayer();
        Transform followTarget = player != null ? player.transform : null;

        if (combat == null || combat.EnemiesInCombat.Count == 0)
        {
            character?.ClearCombatTarget();
            character?.SetFollowTarget(followTarget);
            return;
        }

        _targetUpdateTimer += Time.deltaTime;
        if (_targetUpdateTimer >= Machine.TargetUpdateInterval)
        {
            _targetUpdateTimer = 0f;
            _currentTarget = FindNearestEnemy();
        }

        if (_currentTarget != null && (_currentTarget.Model == null || _currentTarget.Model.IsDead))
            _currentTarget = null;

        if (_currentTarget != null)
        {
            float dist = Vector3.Distance(Machine.transform.position, _currentTarget.transform.position);
            character?.SetCombatTarget(_currentTarget.transform, Machine.AttackRange);

            if (dist <= Machine.AttackRange && character?.StateMachine != null && character.StateMachine.IsIdle)
                character.RequestAttack();
        }
        else
        {
            character?.ClearCombatTarget();
            character?.SetFollowTarget(followTarget);
        }
    }

    public override void Exit()
    {
        _currentTarget = null;
    }

    private Enemy FindNearestEnemy()
    {
        var combat = Machine.CombatController;
        if (combat == null || combat.EnemiesInCombat.Count == 0) return null;

        Vector3 myPos = Machine.transform.position;
        Enemy nearest = null;
        float nearestSq = float.MaxValue;

        foreach (var e in combat.EnemiesInCombat)
        {
            if (e == null || e.Model == null || e.Model.IsDead) continue;

            float sq = (e.transform.position - myPos).sqrMagnitude;
            if (sq < nearestSq)
            {
                nearestSq = sq;
                nearest = e;
            }
        }

        return nearest;
    }
}
