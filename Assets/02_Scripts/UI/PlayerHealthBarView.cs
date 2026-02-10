using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어 체력바 (스크린 공간). Model은 Player.Initialize()에서 주입. Fill Image는 Image Type = Filled, Fill Method = Horizontal 권장.
/// </summary>
public class PlayerHealthBarView : MonoBehaviour
{
    [SerializeField] private Image _fillImage;

    private PlayerModel _model;

    /// <summary>Player에서 Model 주입 시 호출.</summary>
    public void Initialize(PlayerModel model)
    {
        if (_model != null)
            _model.OnHpChanged -= Refresh;

        _model = model;
        if (_model == null || _fillImage == null) return;

        _model.OnHpChanged += Refresh;
        Refresh(_model.CurrentHp, _model.MaxHp);
    }

    private void OnDestroy()
    {
        if (_model != null)
            _model.OnHpChanged -= Refresh;
    }

    private void Refresh(int currentHp, int maxHp)
    {
        if (_fillImage == null) return;
        _fillImage.fillAmount = maxHp > 0 ? (float)currentHp / maxHp : 0f;
    }
}
