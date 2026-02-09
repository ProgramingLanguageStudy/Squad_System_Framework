using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 월드 공간 체력바 (몬스터/엔티티 머리 위). IDamageable 보유 컴포넌트에 연결.
/// 인스펙터에 PlayerModel 또는 MonsterModel 할당. 카메라 방향으로 빌보드.
/// </summary>
public class WorldHealthBarView : MonoBehaviour
{
    [SerializeField] [Tooltip("IDamageable을 구현한 컴포넌트 (PlayerModel, MonsterModel 등)")]
    private MonoBehaviour _damageableHolder;

    [SerializeField] private Image _fillImage;
    [SerializeField] [Tooltip("사망 시 바 숨김")]
    private bool _hideWhenDead = true;

    private IDamageable _damageable;
    private Transform _cameraTransform;

    private void Start()
    {
        _damageable = _damageableHolder as IDamageable;
        if (_damageable == null || _fillImage == null) return;

        if (Camera.main != null) _cameraTransform = Camera.main.transform;

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
