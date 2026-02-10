using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 월드 공간 체력바 (Enemy/엔티티 머리 위). Model은 Enemy 등 소유자가 Initialize()로 주입. 카메라 방향으로 빌보드.
/// </summary>
public class WorldHealthBarView : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] [Tooltip("사망 시 바 숨김")]
    private bool _hideWhenDead = true;

    private IDamageable _damageable;
    private Transform _cameraTransform;

    /// <summary>Enemy 등 소유자가 Model 주입 시 호출.</summary>
    public void Initialize(IDamageable damageable)
    {
        if (_damageable != null)
            _damageable.OnHpChanged -= Refresh;

        _damageable = damageable;
        if (_damageable == null || _fillImage == null) return;

        if (_cameraTransform == null && Camera.main != null)
            _cameraTransform = Camera.main.transform;

        _damageable.OnHpChanged += Refresh;
        Refresh(_damageable.CurrentHp, _damageable.MaxHp);
    }

    private void OnDestroy()
    {
        if (_damageable != null)
            _damageable.OnHpChanged -= Refresh;
    }

    private void LateUpdate()
    {
        if (_cameraTransform == null && Camera.main != null)
            _cameraTransform = Camera.main.transform;
        if (_cameraTransform != null)
            transform.rotation = Quaternion.LookRotation(_cameraTransform.position - transform.position);
    }

    private void Refresh(int currentHp, int maxHp)
    {
        if (_fillImage == null) return;
        _fillImage.fillAmount = maxHp > 0 ? (float)currentHp / maxHp : 0f;

        if (_hideWhenDead && currentHp <= 0 && gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}
