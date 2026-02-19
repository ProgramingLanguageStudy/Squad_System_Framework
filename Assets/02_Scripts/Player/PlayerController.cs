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
    [SerializeField] [Tooltip("세이브 없을 때 조종할 캐릭터. 플레이 모드 시작 시 이 캐릭터를 currentControlled로 설정")]
    private Character _defaultControlled;
    [SerializeField] [Tooltip("비면 교체 불가. 있으면 _defaultControlled + 스폰된 동료 목록으로 순환 교체")]
    private SquadController _squadController;
    [SerializeField] [Tooltip("세이브/로드 시 DataManager에 등록. 있으면 Initialize 시 주입")]
    private PlayerSaveHandler _saveHandler;

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

    private void Awake()
    {
        if (_defaultControlled != null)
        {
            SetCurrentControlled(_defaultControlled);
            _defaultControlled.SetAsPlayer();
        }

        // TODO: 세이브 로드 시 playerCharacterId로 Character 찾아서 SetCurrentControlled
    }

    /// <summary>분대원 순환 교체. 다음 캐릭터로 조종 대상 전환. 교체되면 true.</summary>
    public bool SwapSquad()
    {
        if (_currentControlled == null || _defaultControlled == null) return false;

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
        var list = new List<Character> { _defaultControlled };
        if (_squadController != null)
            list.AddRange(_squadController.Characters);
        return list;
    }

    public void Initialize()
    {
        if (_currentControlled == null && _defaultControlled != null)
            SetCurrentControlled(_defaultControlled);
        if (_saveHandler == null)
            _saveHandler = GetComponent<PlayerSaveHandler>();
        if (_saveHandler != null)
            _saveHandler.SetPlayerController(this);

        if (_currentControlled == null)
        {
            Debug.LogWarning("[PlayerController] Character가 할당되지 않았습니다. 인스펙터에서 defaultControlled를 설정해 주세요.");
            return;
        }

        _currentControlled.Initialize();
    }

    /// <summary>현재 위치·회전·체력을 PlayerSaveData로 반환. PlayerSaveHandler의 Gather에서 사용.</summary>
    public PlayerSaveData GetSaveData()
    {
        return _currentControlled != null ? _currentControlled.GetSaveData() : new PlayerSaveData();
    }

    /// <summary>로드한 PlayerSaveData를 실제 파츠에 적용. PlayerSaveHandler의 Apply에서 사용.</summary>
    public void ApplySaveData(PlayerSaveData data)
    {
        _currentControlled?.ApplySaveData(data);
    }
}
