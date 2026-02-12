using UnityEngine;

/// <summary>
/// 데이터(PlayerData 등)에서 불러오는 기본 스탯. 값 타입으로 복사·저장용.
/// </summary>
[System.Serializable]
public struct PlayerBaseStats
{
    public int maxHp;
    public float moveSpeed;
    public int attackPower;
    public float attackSpeed;
    public int defense;

    /// <summary>PlayerData가 null이면 기본값 반환.</summary>
    public static PlayerBaseStats From(PlayerData data)
    {
        if (data == null)
            return new PlayerBaseStats { maxHp = 100, moveSpeed = 6f, attackPower = 10, attackSpeed = 1f, defense = 0 };

        return new PlayerBaseStats
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
/// 장비·버프 등으로 붙는 보정값. 기본 스탯에 더해서 최종 스탯 계산. (가산만, 나중에 퍼센트 확장 가능)
/// </summary>
[System.Serializable]
public struct PlayerStatModifier
{
    public int maxHp;
    public float moveSpeed;
    public int attackPower;
    public float attackSpeed;
    public int defense;

    public PlayerStatModifier Add(PlayerStatModifier other)
    {
        return new PlayerStatModifier
        {
            maxHp = maxHp + other.maxHp,
            moveSpeed = moveSpeed + other.moveSpeed,
            attackPower = attackPower + other.attackPower,
            attackSpeed = attackSpeed + other.attackSpeed,
            defense = defense + other.defense
        };
    }

    public PlayerStatModifier Subtract(PlayerStatModifier other)
    {
        return new PlayerStatModifier
        {
            maxHp = maxHp - other.maxHp,
            moveSpeed = moveSpeed - other.moveSpeed,
            attackPower = attackPower - other.attackPower,
            attackSpeed = attackSpeed - other.attackSpeed,
            defense = defense - other.defense
        };
    }
}
