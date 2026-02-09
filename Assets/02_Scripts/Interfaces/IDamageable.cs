using System;

/// <summary>
/// 데미지를 받을 수 있는 엔티티 (플레이어, 몬스터 등).
/// Model 비교 기반 전투 시 데미지 적용·체력바 구독에 사용.
/// </summary>
public interface IDamageable
{
    int CurrentHp { get; }
    int MaxHp { get; }
    void TakeDamage(int amount);
    /// <summary>체력 변경 시. (currentHp, maxHp)</summary>
    event Action<int, int> OnHpChanged;
}
