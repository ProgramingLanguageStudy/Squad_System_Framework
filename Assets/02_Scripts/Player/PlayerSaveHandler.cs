using UnityEngine;

/// <summary>
/// 플레이어 세이브/로드 담당. ISaveHandler 구현. PlayerController가 보유하고 SetPlayerController(this)로 주입.
/// Gather/Apply는 PlayerController.GetSaveData / ApplySaveData 직접 호출.
/// </summary>
public class PlayerSaveHandler : MonoBehaviour, ISaveHandler
{
    private PlayerController _playerController;

    /// <summary>PlayerController가 Initialize 시 호출. 주입 전에는 Gather/Apply 동작 안 함.</summary>
    public void SetPlayerController(PlayerController controller)
    {
        _playerController = controller;
    }

    private void OnEnable()
    {
        if (GameManager.Instance?.DataManager != null)
            GameManager.Instance.DataManager.Register(this);
    }

    private void OnDisable()
    {
        if (GameManager.Instance?.DataManager != null)
            GameManager.Instance.DataManager.Unregister(this);
    }

    public void Gather(SaveData data)
    {
        if (data?.player == null) return;
        if (_playerController == null) _playerController = GetComponent<PlayerController>();
        if (_playerController == null) return;
        data.player = _playerController.GetSaveData();
    }

    public void Apply(SaveData data)
    {
        if (data?.player == null) return;
        if (_playerController == null) _playerController = GetComponent<PlayerController>();
        if (_playerController == null) return;
        _playerController.ApplySaveData(data.player);
    }
}
