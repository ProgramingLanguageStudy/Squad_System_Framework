using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 조종 담당. 현재 조종 중인 Character를 관리하고, 입력·세이브를 위임.
/// 별도 GameObject에 두고, Q(SquadSwap) 입력 시 분대원 순환 교체.
/// </summary>
[DefaultExecutionOrder(-10)]
public class PlayerController : MonoBehaviour, IInteractReceiver
{
    [Header("----- 조종 대상 -----")]
    [SerializeField] [Tooltip("분대 스폰. DefaultPlayer가 세이브 없을 때 조종 대상. 반드시 할당")]
    private SquadController _squadController;

    private Character _currentControlled;

    public Character CurrentControlled => _currentControlled;
    public CharacterModel Model => _currentControlled?.Model;
    public CharacterMover Mover => _currentControlled?.Mover;
    public CharacterAnimator Animator => _currentControlled?.Animator;
    public CharacterInteractor Interactor => _currentControlled?.Interactor;
    public CharacterAttacker Attacker => _currentControlled?.Attacker;
    public CharacterStateMachine StateMachine => _currentControlled?.StateMachine;

    public bool CanMove { get; set; } = true;

    /// <summary>조종 대상 변경 시 발행. PlayScene 등에서 구독해 chase/follow/인벤토리 등 갱신.</summary>
    public event Action<Character> OnCurrentControlledChanged;

    /// <summary>조종 대상 교체 (분대 교체 등).</summary>
    public void SetCurrentControlled(Character character)
    {
        _currentControlled = character;
        OnCurrentControlledChanged?.Invoke(character);
    }

    public void RequestAttack()
    {
        _currentControlled?.RequestAttack();
    }

    public void Teleport(Vector3 worldPosition)
    {
        _currentControlled?.Teleport(worldPosition);
    }

    public void Teleport(Transform destination)
    {
        _currentControlled?.Teleport(destination);
    }

    public void SetCharacterControllerEnabled(bool enabled)
    {
        _currentControlled?.SetCharacterControllerEnabled(enabled);
    }

    /// <summary>분대원 순환 교체. 다음 캐릭터로 조종 대상 전환. 교체되면 true.</summary>
    public bool SwapSquad()
    {
        if (_currentControlled == null || _squadController == null) return false;

        var list = BuildSwappableList();
        if (list == null || list.Count == 0) return false;

        int idx = list.IndexOf(_currentControlled);
        if (idx < 0) idx = 0;
        int nextIdx = (idx + 1) % list.Count;
        var next = list[nextIdx];
        if (next == _currentControlled) return false;

        _currentControlled.SetAsCompanion(next.transform);
        next.SetAsPlayer();
        SetCurrentControlled(next);
        return true;
    }

    private List<Character> BuildSwappableList()
    {
        return _squadController != null
            ? new List<Character>(_squadController.Characters)
            : new List<Character>();
    }

    public void Initialize()
    {
        if (_currentControlled == null && _squadController != null)
        {
            var defaultPlayer = _squadController.DefaultPlayer;
            if (defaultPlayer != null)
                SetCurrentControlled(defaultPlayer);
        }

        if (_currentControlled == null)
        {
            Debug.LogWarning("[PlayerController] Character가 없습니다. SquadController._initialSquad에 캐릭터를 추가하세요.");
            return;
        }

        _currentControlled.Initialize();
    }
}
