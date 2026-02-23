using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적 팀. 멤버의 OnEnteringCombat 구독 → 전원 Chase, Combat 등록.
/// </summary>
public class EnemyTeam : MonoBehaviour
{
    private CombatController _combatController;
    private readonly List<Enemy> _members = new List<Enemy>();

    public IReadOnlyList<Enemy> Members => _members;

    public void Initialize(CombatController combatController)
    {
        _combatController = combatController;
    }

    public void AddMember(Enemy enemy)
    {
        if (enemy == null || _members.Contains(enemy)) return;
        _members.Add(enemy);
        enemy.OnEnteringCombat += HandleMemberEnteringCombat;
        enemy.OnDestroyed += RemoveMember;
    }

    public void RemoveMember(Enemy enemy)
    {
        if (enemy == null) return;
        enemy.OnEnteringCombat -= HandleMemberEnteringCombat;
        enemy.OnDestroyed -= RemoveMember;
        _members.Remove(enemy);
    }

    private void HandleMemberEnteringCombat(Transform chaseTarget)
    {
        foreach (var m in _members)
        {
            if (m == null || m.Model == null || m.Model.IsDead) continue;
            m.SetChaseTarget(chaseTarget);
            m.StateMachine.RequestChase();
        }
    }
}
