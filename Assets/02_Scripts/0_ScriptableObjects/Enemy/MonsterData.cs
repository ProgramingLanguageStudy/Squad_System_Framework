using UnityEngine;

/// <summary>
/// 몬스터 기초 스탯 정의 (디자인 타임). SO로 생성 후 MonsterModel에 할당.
/// </summary>
[CreateAssetMenu(fileName = "MonsterData", menuName = "Enemy/Monster Data")]
public class MonsterData : ScriptableObject
{
    [Header("생존")]
    [Tooltip("최대 체력")]
    public int maxHp = 50;

    [Header("공격")]
    [Tooltip("공격력 (데미지 계산 시 사용)")]
    public int attackPower = 5;

    [Header("방어")]
    [Tooltip("방어력 (받는 데미지 감소)")]
    public int defense = 0;
}
