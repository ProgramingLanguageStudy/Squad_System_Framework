using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격 관련 로직. HitboxController + 감지 시 데미지 적용.
/// 애니 이벤트: Animation_BeginHitWindow, Animation_EndHitWindow, Animation_OnAttackEnded.
/// </summary>
public class CharacterAttacker : MonoBehaviour
{
    [SerializeField] private HitboxController _hitboxController;

    private CharacterModel _ownerModel;
    private CharacterStateMachine _stateMachine;
    private readonly HashSet<IDamageable> _hitThisAttack = new HashSet<IDamageable>();

    public void Initialize(CharacterStateMachine stateMachine, CharacterModel ownerModel)
    {
        _stateMachine = stateMachine;
        _ownerModel = ownerModel;

        if (_hitboxController == null)
            _hitboxController = GetComponentInChildren<HitboxController>(true);
        if (_hitboxController == null)
            Debug.LogWarning($"[CharacterAttacker] {gameObject.name}: HitboxController를 찾을 수 없습니다. 자식 계층 어딘가에 있어야 합니다.");
    }

    private void OnEnable()
    {
        if (_hitboxController != null)
            _hitboxController.OnDamageableDetected += ApplyDamage;
    }

    private void OnDisable()
    {
        if (_hitboxController != null)
            _hitboxController.OnDamageableDetected -= ApplyDamage;
    }

    private void ApplyDamage(IDamageable target)
    {
        if (target == null || _ownerModel == null) return;
        if (ReferenceEquals(target, _ownerModel)) return;
        if (_hitThisAttack.Contains(target)) return;

        _hitThisAttack.Add(target);
        target.TakeDamage(_ownerModel.AttackPower);
    }

    public void OnAttackStarted()
    {
        _hitThisAttack.Clear();
    }

    public void Animation_BeginHitWindow()
    {
        _hitboxController?.EnableHit();
    }

    public void Animation_EndHitWindow()
    {
        _hitboxController?.DisableHit();
    }

    public void Animation_OnAttackEnded()
    {
        _hitboxController?.DisableHit();
        _stateMachine?.RequestIdle();
    }

    public void EndAttackCleanup()
    {
        _hitboxController?.DisableHit();
    }
}
