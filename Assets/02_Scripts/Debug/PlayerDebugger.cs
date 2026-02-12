using UnityEngine;

/// <summary>플레이어 디버그/치트용. Hierarchy의 Debuggers 등에 붙이고, 인스펙터에서 반드시 Player 참조를 할당하세요.</summary>
public class PlayerDebugger : MonoBehaviour
{
    [SerializeField] [Tooltip("반드시 인스펙터에서 할당하세요. 참조 없으면 버튼 동작하지 않음.")]
    private Player _player;

    public Player PlayerRef => _player;
}
