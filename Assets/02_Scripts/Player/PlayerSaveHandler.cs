using UnityEngine;

/// <summary>
/// 플레이어 세이브/로드 담당. ISaveHandler 구현. Player가 보유하고 SetPlayer(this)로 주입.
/// Gather/Apply는 Player.GetSaveData / ApplySaveData 직접 호출.
/// </summary>
[RequireComponent(typeof(Player))]
public class PlayerSaveHandler : MonoBehaviour, ISaveHandler
{
    private Player _player;

    /// <summary>Player가 Initialize 시 호출. 주입 전에는 Gather/Apply 동작 안 함.</summary>
    public void SetPlayer(Player player)
    {
        _player = player;
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
        if (_player == null) _player = GetComponent<Player>();
        if (_player == null) return;
        data.player = _player.GetSaveData();
    }

    public void Apply(SaveData data)
    {
        if (data?.player == null) return;
        if (_player == null) _player = GetComponent<Player>();
        if (_player == null) return;
        _player.ApplySaveData(data.player);
    }
}
