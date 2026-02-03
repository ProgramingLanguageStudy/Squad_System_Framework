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

        // [����] ���⼭ Hide()�� �θ��� ���, 
        // ���̾��Ű â���� �ƿ� TooltipPanel�� ���δ� �� �����մϴ�.
    }

    private void Update()
    {
        // _tooltipPanel�� �Ҵ���� �ʾҰų� ���������� ���� �� ��
        if (_tooltipPanel == null || !_tooltipPanel.activeSelf) return;

        _rectTransform.position = Mouse.current.position.ReadValue() + new Vector2(10, 10);
    }

    public void Show(string itemName, string description)
    {
        if (_tooltipPanel == null) return; // ��� �ڵ�

        _nameText.text = itemName;
        _descText.text = description;
        _tooltipPanel.SetActive(true);
    }

    public void Hide()
    {
        // 40�� �� ���� ����: �г��� �Ҵ�Ǿ� �ִ��� Ȯ��
        if (_tooltipPanel != null)
        {
            _tooltipPanel.SetActive(false);
        }
    }
}