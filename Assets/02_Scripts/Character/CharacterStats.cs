using UnityEngine;

/// <summary>
/// 데이터(CharacterData)에서 불러오는 기본 스탯. 값 타입으로 복사·저장용.
/// </summary>
[System.Serializable]
public struct CharacterBaseStats
{
    public int maxHp;
    public float moveSpeed;
    public int attackPower;
    public float attackSpeed;
    public int defense;

    public static CharacterBaseStats From(CharacterData data)
    {
        if (data == null)
            return new CharacterBaseStats { maxHp = 100, moveSpeed = 6f, attackPower = 10, attackSpeed = 1f, defense = 0 };

        return new CharacterBaseStats
        {
            maxHp = data.maxHp,
            moveSpeed = data.moveSpeed,
            attackPower = data.attackPower,
            attackSpeed = data.attackSpeed,
            defense = data.defense
        };
    }
}

/// <summary>
/// 장비·버프 등으로 붙는 보정값. 기본 스탯에 더해서 최종 스탯 계산.
/// </summary>
[System.Serializable]
public struct StatModifier
{
    public int maxHp;
    public float moveSpeed;
    public int attackPower;
    public float attackSpeed;
    public int defense;

    public StatModifier Add(StatModifier other)
    {
        return new StatModifier
        {
            maxHp = maxHp + other.maxHp,
            moveSpeed = moveSpeed + other.moveSpeed,
            attackPower = attackPower + other.attackPower,
            attackSpeed = attackSpeed + other.attackSpeed,
            defense = defense + other.defense
        };
    }

    public StatModifier Subtract(StatModifier other)
    {
        return new StatModifier
        {
            maxHp = maxHp - other.maxHp,
            moveSpeed = moveSpeed - other.moveSpeed,
            attackPower = attackPower - other.attackPower,
            attackSpeed = attackSpeed - other.attackSpeed,
            defense = defense - other.defense
        };
    }
}
