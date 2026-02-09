using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어 체력바 (스크린 공간). Player.Model.OnHpChanged 구독해 Fill 갱신.
/// Fill Image는 Image Type = Filled, Fill Method = Horizontal 권장.
/// </summary>
public class PlayerHealthBarView : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Image _fillImage;

    private void Start()
    {
        if (_player == null || _player.Model == null || _fillImage == null) return;

        _player.Model.OnHpChanged += Refresh;
        Refresh(_player.Model.CurrentHp, _player.Model.MaxHp);
    }

    private void OnDestroy()
    {
        if (_player != null && _player.Model != null)
            _player.Model.OnHpChanged -= Refresh;
    }

    private void Refresh(int currentHp, int maxHp)
    {
        if (_fillImage == null) return;
        _fillImage.fillAmount = maxHp > 0 ? (float)currentHp / maxHp : 0f;
    }
}
