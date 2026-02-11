using UnityEngine;

/// <summary>
/// 부활 요청 시 마을 포탈 위치로 이동·체력 회복·Idle 전환. Player는 포탈을 모름.
/// </summary>
public class RespawnController : MonoBehaviour
{
    [SerializeField] [Tooltip("부활 위치. 마을 포탈 등")]
    private Portal _respawnPortal;

    private void OnEnable()
    {
        GameEvents.OnRespawnRequested += HandleRespawnRequested;
    }

    private void OnDisable()
    {
        GameEvents.OnRespawnRequested -= HandleRespawnRequested;
    }

    private void HandleRespawnRequested(Player player)
    {
        if (player == null) return;

        if (_respawnPortal != null)
            player.Teleport(_respawnPortal.ArrivalPosition);
        player.Model?.Heal(player.Model.MaxHp);
        player.Animator?.ResetToIdle(); // 누운 상태 해제
        player.StateMachine?.RequestIdle();
    }
}
