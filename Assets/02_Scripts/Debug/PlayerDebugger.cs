using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어·캐릭터 디버그·검증. Hierarchy의 Debuggers 등에 붙이고, 인스펙터에서 PlayerController 참조 할당.
/// </summary>
public class PlayerDebugger : MonoBehaviour
{
    [SerializeField] [Tooltip("비어 있으면 씬에서 FindObjectOfType으로 탐색")]
    private PlayerController _playerController;

    public PlayerController PlayerRef => _playerController;

    private void OnValidate()
    {
        if (_playerController == null)
            _playerController = FindAnyObjectByType<PlayerController>();
    }

    /// <summary>PlayerController·Character 부품 구성을 검증하고 누락 항목을 로그.</summary>
    public bool ValidateSetup(out List<string> issues)
    {
        issues = new List<string>();

        if (_playerController == null)
        {
            _playerController = FindAnyObjectByType<PlayerController>();
            if (_playerController == null)
            {
                issues.Add("PlayerController를 찾을 수 없습니다. 씬에 PlayerController가 있는지 확인하세요.");
                return false;
            }
        }

        var c = _playerController.CurrentControlled;
        if (c == null)
            issues.Add("PlayerController에 currentControlled(Character)가 없습니다. defaultControlled를 할당하세요.");

        if (c == null)
            return false;

        if (c.Model == null) issues.Add("Character: Model 없음");
        if (c.Animator == null) issues.Add("Character: Animator(CharacterAnimator) 없음");
        if (c.StateMachine == null) issues.Add("Character: StateMachine 없음");

        // 플레이어 조종 시 필요한 것들 (동료는 일부 없을 수 있음)
        bool hasMover = c.Mover != null;
        bool hasFollowMover = c.GetComponent<CharacterFollowMover>() != null;
        if (!hasMover && !hasFollowMover)
            issues.Add("Character: Mover 또는 FollowMover 중 하나 필요");

        if (c.Mover != null && c.GetComponent<CharacterController>() == null)
            issues.Add("Character: Mover 사용 시 CharacterController 필요");

        if (c.GetComponent<CharacterFollowMover>() != null && c.GetComponent<UnityEngine.AI.NavMeshAgent>() == null)
            issues.Add("Character: FollowMover 사용 시 NavMeshAgent 필요");

        return issues.Count == 0;
    }

    [ContextMenu("Validate Setup (Log)")]
    private void ValidateAndLog()
    {
        if (ValidateSetup(out var issues))
        {
            Debug.Log("[PlayerDebugger] 검증 통과: 부품 구성 정상");
        }
        else
        {
            foreach (var msg in issues)
                Debug.LogWarning($"[PlayerDebugger] {msg}");
        }
    }
}
