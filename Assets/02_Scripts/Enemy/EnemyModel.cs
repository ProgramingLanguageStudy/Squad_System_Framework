using System;
using UnityEngine;

/// <summary>
/// Enemy 런타임 데이터. EnemyData 기반으로 현재 HP 등 상태 보관.
/// 전투 시 IDamageable/IAttackPowerSource로 데미지 주고받기에 사용.
/// </summary>
public class EnemyModel : MonoBehaviour, IDamageable, IAttackPowerSource
{
    [SerializeField] private EnemyData _data;

    private int _currentHp;

    public EnemyData Data => _data;
    public int CurrentHp => _currentHp;
    public int MaxHp => _data != null ? _data.maxHp : 50;
    public int AttackPower => _data != null ? _data.attackPower : 5;
    public int Defense => _data != null ? _data.defense : 0;

    public event Action<int, int> OnHpChanged;

    public void Initialize()
    {
        _currentHp = MaxHp;
        OnHpChanged?.Invoke(_currentHp, MaxHp);
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        int reduced = Mathf.Max(0, amount - Defense);
        _currentHp = Mathf.Max(0, _currentHp - reduced);
        OnHpChanged?.Invoke(_currentHp, MaxHp);
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        _currentHp = Mathf.Min(MaxHp, _currentHp + amount);
        OnHpChanged?.Invoke(_currentHp, MaxHp);
    }

    /// <summary>사망 여부. 체력바 숨김 등에 사용.</summary>
    public bool IsDead => _currentHp <= 0;

    // --- AI 유효값 (Data 기반. 버프 등은 여기서 적용 예정) ---
    public float PatrolSpeed => _data != null ? _data.patrolSpeed : 1.5f;
    public float PatrolRadius => _data != null ? _data.patrolRadius : 5f;
    public float ArriveThreshold => _data != null ? _data.arriveThreshold : 0.5f;
    public float PatrolWalkDurationMin => _data != null ? _data.patrolWalkDurationMin : 2f;
    public float PatrolWalkDurationMax => _data != null ? _data.patrolWalkDurationMax : 3f;
    public float PatrolIdleDuration => _data != null ? _data.patrolIdleDuration : 1f;
    public float ChaseSpeed => _data != null ? _data.chaseSpeed : 4f;
    public float DetectionRadius => _data != null ? _data.detectionRadius : 10f;
    public float AttackRadius => _data != null ? _data.attackRadius : 2f;
    public float ChaseLoseRadius => _data != null ? _data.chaseLoseRadius : 15f;
    public float AttackDuration => _data != null ? _data.attackDuration : 0.6f;
}
