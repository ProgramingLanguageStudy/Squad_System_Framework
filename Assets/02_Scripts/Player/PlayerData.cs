using UnityEngine;

/// <summary>
/// 플레이어 기초 스탯 정의 (디자인 타임). SO로 생성 후 PlayerModel에 할당.
/// </summary>
[CreateAssetMenu(fileName = "PlayerData", menuName = "Player/Data")]
public class PlayerData : ScriptableObject
{
    [Header("생존")]
    [Tooltip("최대 체력")]
    public int maxHp = 100;

    [Header("이동")]
    [Tooltip("이동 속도 (NavMeshAgent 등에 사용)")]
    public float moveSpeed = 6f;

    [Header("공격")]
    [Tooltip("공격력 (데미지 계산 시 사용)")]
    public int attackPower = 10;
    [Tooltip("공격 속도 배율 (1 = 기본. 애니/쿨다운에 적용)")]
    [Range(0.5f, 2f)]
    public float attackSpeed = 1f;

    [Header("방어")]
    [Tooltip("방어력 (받는 데미지 감소 등)")]
    public int defense = 0;
}
