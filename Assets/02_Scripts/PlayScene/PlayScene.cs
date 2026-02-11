using UnityEngine;

/// <summary>
/// 플레이 씬 조율. InputHandler·Player 참조 보유, 입력 이벤트/값을 플레이어·GameEvents로 연결.
/// 이동, Interact, 인벤토리 키만 연결. 퀘스트는 트래커형이라 키 연결 없음.
/// </summary>
public class PlayScene : MonoBehaviour
{
    [SerializeField] private InputHandler _inputHandler;
    [SerializeField] private Player _player;
    [SerializeField] [Tooltip("비면 주입 안 함. 있으면 Player를 chase target으로 주입")]
    private EnemySpawner _enemySpawner;

    private void Awake()
    {
        if (_inputHandler == null)
        {
            Debug.LogWarning("[PlayScene] InputHandler가 할당되지 않았습니다. 인스펙터에서 할당해 주세요.");
            return;
        }
        if (_player == null)
        {
            Debug.LogWarning("[PlayScene] Player가 할당되지 않았습니다. 인스펙터에서 할당해 주세요.");
            return;
        }

        _player.Initialize();
        _enemySpawner?.Initialize(_player.transform);
    }

    private void OnEnable()
    {
        if (_inputHandler == null || _player == null) return;

        _inputHandler.OnInteractPerformed += HandleInteract;
        _inputHandler.OnInventoryPerformed += HandleInventoryKey;
        _inputHandler.OnAttackPerformed += HandleAttack;
    }

    private void OnDisable()
    {
        if (_inputHandler == null) return;

        _inputHandler.OnInteractPerformed -= HandleInteract;
        _inputHandler.OnInventoryPerformed -= HandleInventoryKey;
        _inputHandler.OnAttackPerformed -= HandleAttack;
    }

    private void Update()
    {
        if (_inputHandler == null || _player == null) return;

        if (!_player.CanMove)
            return;

        _player.Mover.Move(_inputHandler.MoveInput);
    }

    private void HandleInteract()
    {
        _player?.Interactor?.TryInteract();
    }

    private void HandleInventoryKey()
    {
        GameEvents.OnInventoryKeyPressed?.Invoke();
    }

    private void HandleAttack()
    {
        _player?.RequestAttack();
    }
}
