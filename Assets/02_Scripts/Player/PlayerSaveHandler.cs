using UnityEngine;

/// <summary>
/// 플레이어 세이브/로드 담당. ISaveHandler 구현. OnEnable에서 DataManager에 등록, OnDisable에서 해제.
/// Player와 같은 GameObject에 두고, Player·PlayerModel 참조로 data.player만 채우고 적용.
/// </summary>
[RequireComponent(typeof(Player))]
public class PlayerSaveHandler : MonoBehaviour, ISaveHandler
{
    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
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
        if (data?.player == null || _player == null) return;

        var t = _player.transform;
        data.player.position = t.position;
        data.player.rotationY = t.eulerAngles.y;
        data.player.currentHp = _player.Model != null ? _player.Model.CurrentHp : 0;
    }

    public void Apply(SaveData data)
    {
        if (data?.player == null || _player == null) return;

        _player.Teleport(data.player.position);
        _player.transform.eulerAngles = new Vector3(0f, data.player.rotationY, 0f);
        _player.Model?.SetCurrentHpForLoad(data.player.currentHp);
    }
}
