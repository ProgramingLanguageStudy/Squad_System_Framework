using TMPro;
using UnityEngine.InputSystem;
using UnityEngine;

public class TooltipUI : UISingleton<TooltipUI>
{
    [SerializeField] private GameObject _tooltipPanel;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descText;

    private RectTransform _rectTransform;

    protected override void Awake()
    {
        base.Awake();
        _rectTransform = GetComponent<RectTransform>();

        // [참고] 여기서 Hide()를 호출해 두면,
        // 게임 시작 시 창이 닫힌 상태로 TooltipPanel이 보이지 않게 합니다.
    }

    private void Update()
    {
        // _tooltipPanel이 비활성화되어 있으면 위치 갱신 생략
        if (_tooltipPanel == null || !_tooltipPanel.activeSelf) return;

        _rectTransform.position = Mouse.current.position.ReadValue() + new Vector2(10, 10);
    }

    public void Show(string itemName, string description)
    {
        if (_tooltipPanel == null) return; // 방어 코드

        _nameText.text = itemName;
        _descText.text = description;
        _tooltipPanel.SetActive(true);
    }

    public void Hide()
    {
        // 40줄 대신 간단히: 패널이 비활성화되어 있는지 확인
        if (_tooltipPanel != null)
        {
            _tooltipPanel.SetActive(false);
        }
    }
}
