/// <summary>
/// 공격력 제공처. HitDamageApplier 등이 데미지량 계산 시 사용.
/// </summary>
public interface IAttackPowerSource
{
    int AttackPower { get; }
}
