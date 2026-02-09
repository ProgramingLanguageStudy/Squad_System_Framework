using System;
using UnityEngine;

/// <summary>
/// 몬스터 런타임 데이터. MonsterData 기반으로 현재 HP 등 상태 보관.
/// 전투 시 IDamageable/IAttackPowerSource로 데미지 주고받기에 사용.
/// </summary>
public class MonsterModel : MonoBehaviour, IDamageable, IAttackPowerSource
{
    [SerializeField] private MonsterData _data;

    private int _currentHp;

    public MonsterData Data => _data;
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
}
