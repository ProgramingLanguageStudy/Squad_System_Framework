using System;
using UnityEngine;

/// <summary>
/// 플레이어 런타임 데이터. PlayerData 기반으로 현재 HP 등 상태 보관.
/// Player가 컴포넌트로 보유하고, Mover/Attacker 등이 스탯 참조 시 사용.
/// 전투 시 IDamageable/IAttackPowerSource로 통일 처리.
/// </summary>
public class PlayerModel : MonoBehaviour, IDamageable, IAttackPowerSource
{
    [SerializeField] private PlayerData _data;

    private int _currentHp;

    /// <summary>기초 스탯 에셋. 비어 있으면 기본값 사용.</summary>
    public PlayerData Data => _data;

    /// <summary>현재 체력. 런타임에 변경됨.</summary>
    public int CurrentHp => _currentHp;
    /// <summary>최대 체력. Data가 있으면 Data.maxHp, 없으면 100.</summary>
    public int MaxHp => _data != null ? _data.maxHp : 100;

    /// <summary>이동 속도. Data가 있으면 Data.moveSpeed, 없으면 6.</summary>
    public float MoveSpeed => _data != null ? _data.moveSpeed : 6f;
    /// <summary>공격력. Data가 있으면 Data.attackPower, 없으면 10.</summary>
    public int AttackPower => _data != null ? _data.attackPower : 10;
    /// <summary>공격 속도 배율.</summary>
    public float AttackSpeed => _data != null ? _data.attackSpeed : 1f;
    /// <summary>방어력.</summary>
    public int Defense => _data != null ? _data.defense : 0;

    /// <summary>사망 등 체력 변경 시. (currentHp, maxHp)</summary>
    public event Action<int, int> OnHpChanged;

    public void Initialize()
    {
        _currentHp = MaxHp;
        OnHpChanged?.Invoke(_currentHp, MaxHp);
    }

    /// <summary>데미지 적용. 실제 감소량은 방어력 등 적용 후. 나중에 공식 넣을 때 사용.</summary>
    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        int reduced = Mathf.Max(0, amount - Defense);
        _currentHp = Mathf.Max(0, _currentHp - reduced);
        OnHpChanged?.Invoke(_currentHp, MaxHp);
    }

    /// <summary>체력 회복. (나중에 힐 로직 확장용)</summary>
    public void Heal(int amount)
    {
        if (amount <= 0) return;
        _currentHp = Mathf.Min(MaxHp, _currentHp + amount);
        OnHpChanged?.Invoke(_currentHp, MaxHp);
    }
}
