using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이 화면 전체 UI. 체력바 등. PlayScene이 보유하고 Model.OnHpChanged 구독 후 RefreshHealth 호출.
/// 동료 체력 UI는 추후 추가 예정.
/// </summary>
public class PlaySceneView : MonoBehaviour
{
    [Header("----- 체력바 (현재 조종 캐릭터) -----")]
    [SerializeField] [Tooltip("Image Type = Filled, Fill Method = Horizontal 권장")]
    private Image _healthFillImage;

    /// <summary>PlayScene이 Model.OnHpChanged 구독 후 호출. 현재 조종 캐릭터 체력 표시.</summary>
    public void RefreshHealth(int currentHp, int maxHp)
    {
        if (_healthFillImage == null) return;
        _healthFillImage.fillAmount = maxHp > 0 ? (float)currentHp / maxHp : 0f;
    }
}
