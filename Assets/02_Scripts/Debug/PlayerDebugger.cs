using UnityEngine;

/// <summary>플레이어 디버그/치트용. Hierarchy의 Debuggers 등에 붙이고, 인스펙터에서 Player 할당 (비면 플레이 시 Find 시도).</summary>
public class PlayerDebugger : MonoBehaviour
{
    [SerializeField] [Tooltip("비워두면 플레이 모드에서 FindFirstObjectByType으로 찾음")]
    private Player _player;

    public Player PlayerRef => _player;
}
