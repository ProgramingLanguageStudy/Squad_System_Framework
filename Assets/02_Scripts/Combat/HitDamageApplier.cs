using UnityEngine;

/// <summary>
/// Weapon을 통해 감지 결과를 받아 데미지만 적용. 플레이어/몬스터 쪽에 두고 _weapon만 연결.
/// </summary>
public class HitDamageApplier : MonoBehaviour
{
    [SerializeField] [Tooltip("감지 이벤트를 구독할 무기. Player는 Player의 Weapon, 몬스터는 몬스터의 Weapon")]
    private Weapon _weapon;

    [SerializeField] [Tooltip("데미지를 받으면 안 되는 쪽 (본인). PlayerModel 또는 MonsterModel")]
    private MonoBehaviour _ownerDamageable;

    [SerializeField] [Tooltip("공격력 제공. PlayerModel 또는 MonsterModel")]
    private MonoBehaviour _attackPowerSource;

    private void OnEnable()
    {
        if (_weapon != null)
            _weapon.OnDamageableDetected += ApplyIfValid;
    }

    private void OnDisable()
    {
        if (_weapon != null)
            _weapon.OnDamageableDetected -= ApplyIfValid;
    }

    private void ApplyIfValid(IDamageable target)
    {
        if (target == null) return;
        if (_ownerDamageable != null && (target == _ownerDamageable as IDamageable)) return;

        var source = _attackPowerSource as IAttackPowerSource;
        if (source == null) return;

        target.TakeDamage(source.AttackPower);
    }
}
