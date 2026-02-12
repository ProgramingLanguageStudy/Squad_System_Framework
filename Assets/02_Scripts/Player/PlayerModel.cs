using System;
using UnityEngine;

/// <summary>
/// 플레이어 런타임 데이터. PlayerData → PlayerBaseStats, 보정(PlayerStatModifier) 적용 후 최종 스탯 노출.
/// Player가 컴포넌트로 보유하고, Mover/Attacker 등이 스탯 참조 시 사용.
/// </summary>
public class PlayerModel : MonoBehaviour, IDamageable, IAttackPowerSource, IItemUser
{
    [SerializeField] private PlayerData _data;

    private PlayerBaseStats _baseStats;
    private PlayerStatModifier _modifier;
    private int _currentHp;

    private struct BuffEntry
    {
        public PlayerStatModifier Modifier;
        public float RemoveAtTime;
        public BuffEntry(PlayerStatModifier modifier, float removeAtTime) { Modifier = modifier; RemoveAtTime = removeAtTime; }
    }
    private System.Collections.Generic.List<BuffEntry> _timedBuffs = new System.Collections.Generic.List<BuffEntry>();

    /// <summary>기초 스탯 에셋. 비어 있으면 기본값 사용.</summary>
    public PlayerData Data => _data;

    /// <summary>기본 스탯 (데이터에서 로드). 읽기 전용 노출용.</summary>
    public PlayerBaseStats BaseStats => _baseStats;
    /// <summary>현재 보정값 (장비·버프 등). 추가/제거는 AddModifier, RemoveModifier 사용.</summary>
    public PlayerStatModifier Modifier => _modifier;

    /// <summary>현재 체력. 런타임에 변경됨.</summary>
    public int CurrentHp => _currentHp;
    /// <summary>최대 체력. 기본 + 보정, 최소 1.</summary>
    public int MaxHp => Mathf.Max(1, _baseStats.maxHp + _modifier.maxHp);
    /// <summary>사망 여부. 체력 0 이하.</summary>
    public bool IsDead => _currentHp <= 0;

    /// <summary>이동 속도. 기본 + 보정, 최소 0.</summary>
    public float MoveSpeed => Mathf.Max(0f, _baseStats.moveSpeed + _modifier.moveSpeed);
    /// <summary>공격력. 기본 + 보정, 최소 0.</summary>
    public int AttackPower => Mathf.Max(0, _baseStats.attackPower + _modifier.attackPower);
    /// <summary>공격 속도 배율. 기본 + 보정, 최소 0.</summary>
    public float AttackSpeed => Mathf.Max(0f, _baseStats.attackSpeed + _modifier.attackSpeed);
    /// <summary>방어력. 기본 + 보정, 최소 0.</summary>
    public int Defense => Mathf.Max(0, _baseStats.defense + _modifier.defense);

    /// <summary>사망 등 체력 변경 시. (currentHp, maxHp)</summary>
    public event Action<int, int> OnHpChanged;

    public void Initialize()
    {
        _baseStats = PlayerBaseStats.From(_data);
        _modifier = default;
        _currentHp = MaxHp;
        OnHpChanged?.Invoke(_currentHp, MaxHp);
    }

    /// <summary>보정 추가 (장비 장착·버프 등). 같은 보정 제거 시 RemoveModifier로 동일 값 넘기면 됨.</summary>
    public void AddModifier(PlayerStatModifier delta)
    {
        _modifier = _modifier.Add(delta);
        _currentHp = Mathf.Clamp(_currentHp, 0, MaxHp);
        OnHpChanged?.Invoke(_currentHp, MaxHp);
    }

    /// <summary>보정 제거 (장비 해제·버프 해제 등).</summary>
    public void RemoveModifier(PlayerStatModifier delta)
    {
        _modifier = _modifier.Subtract(delta);
        _currentHp = Mathf.Clamp(_currentHp, 0, MaxHp);
        OnHpChanged?.Invoke(_currentHp, MaxHp);
    }

    /// <summary>데미지 적용. 실제 감소량은 방어력 적용 후.</summary>
    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        int reduced = Mathf.Max(0, amount - Defense);
        _currentHp = Mathf.Max(0, _currentHp - reduced);
        OnHpChanged?.Invoke(_currentHp, MaxHp);
    }

    /// <summary>체력 회복. (IItemUser)</summary>
    public void Heal(int amount)
    {
        if (amount <= 0) return;
        _currentHp = Mathf.Min(MaxHp, _currentHp + amount);
        OnHpChanged?.Invoke(_currentHp, MaxHp);
    }

    /// <summary>일시 버프 적용. durationSeconds 후 자동 해제. (IItemUser)</summary>
    public void ApplyBuff(PlayerStatModifier modifier, float durationSeconds)
    {
        if (durationSeconds <= 0f) return;
        AddModifier(modifier);
        _timedBuffs.Add(new BuffEntry(modifier, Time.time + durationSeconds));
    }

    private void Update()
    {
        if (_timedBuffs.Count == 0) return;
        float now = Time.time;
        for (int i = _timedBuffs.Count - 1; i >= 0; i--)
        {
            if (now >= _timedBuffs[i].RemoveAtTime)
            {
                RemoveModifier(_timedBuffs[i].Modifier);
                _timedBuffs.RemoveAt(i);
            }
        }
    }

    /// <summary>세이브 로드 시 체력 복원용. 0~MaxHp로 클램프 후 적용.</summary>
    public void SetCurrentHpForLoad(int value)
    {
        _currentHp = Mathf.Clamp(value, 0, MaxHp);
        OnHpChanged?.Invoke(_currentHp, MaxHp);
    }
}
