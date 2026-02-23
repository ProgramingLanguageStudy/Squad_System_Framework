using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적 팀. 한 명이 전투 진입 시 전원이 Chase. CombatController에 등록.
/// </summary>
public class EnemyTeam : MonoBehaviour
{
    [SerializeField] private CombatController _combatController;

    private readonly List<Enemy> _members = new List<Enemy>();

    public IReadOnlyList<Enemy> Members => _members;

    public void Initialize(CombatController combatController)
    {
        _combatController = combatController;
    }

    public void AddMember(Enemy enemy)
    {
        if (enemy != null && !_members.Contains(enemy))
            _members.Add(enemy);
    }

    /// <summary>멤버가 전투 진입 시 호출. 전원 Chase, Combat 등록.</summary>
    public void OnMemberEnteredCombat(Enemy who, Transform chaseTarget)
    {
        foreach (var m in _members)
        {
            if (m == null || m.Model == null || m.Model.IsDead) continue;
            m.SetChaseTarget(chaseTarget);
            m.StateMachine.RequestChase();
            _combatController?.RegisterInCombat(m);
        }
    }
}
