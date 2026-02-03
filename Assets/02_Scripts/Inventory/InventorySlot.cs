using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler // �巡�� �������̽� �߰�
{
    [Header("----- UI ���� -----")]
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _countText;

    [Header("----- ���� -----")]
    [SerializeField] private int _index; // �� ������ �� ��° �������� (Inspector���� ����)

    // ���� ������ ��� �ִ� ������
    private ItemData _currentItem;
    private int _currentCount;

    // ������Ƽ�� ���� �ܺ�(DragManager)���� Index�� ���� �� �ְ� �մϴ�.
    public int Index => _index;

    // ������ Sprite ���� ���޿� Getter
    public Sprite GetIcon() => _currentItem != null ? _currentItem.Icon : null;

    public void SetIndex(int index)
    {
        _index = index;
        // ���̾��Ű â���� ���� ������ ���ϵ��� �̸��� �ٲ��ָ� �����ϴ�.
        gameObject.name = $"Slot_{index:00}";
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    public void ClearSlot()
    {
        _currentItem = null;
        _currentCount = 0;

        _icon.sprite = null;
        _icon.enabled = false;
        _countText.text = string.Empty;
    }

    /// <summary>
    /// ������ ���� ������Ʈ
    /// </summary>
    public void UpdateSlot(ItemData item, int count)
    {
        if (item == null || count <= 0)
        {
            ClearSlot();
            return;
        }

        _currentItem = item;
        _currentCount = count;

        _icon.sprite = _currentItem.Icon;
        _icon.enabled = true;
        _countText.text = _currentCount > 1 ? _currentCount.ToString() : string.Empty;
    }

    #region ���콺 ������ �̺�Ʈ (����)

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_currentItem != null && TooltipUI.Instance != null)
        {
            TooltipUI.Instance.Show(_currentItem.ItemName, _currentItem.Description);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipUI.Instance != null)
        {
            TooltipUI.Instance.Hide();
        }
    }

    #endregion

    #region �巡�� �̺�Ʈ (��ġ ����)

    public void OnBeginDrag(PointerEventData eventData)
    {
        // �������� ���� ���� �巡�� ����
        if (_currentItem != null && InventoryDragManager.Instance != null)
        {
            InventoryDragManager.Instance.StartDrag(this);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (InventoryDragManager.Instance != null)
        {
            // ���콺 ��ġ�� �ǽð� ���� (eventData.position ���)
            InventoryDragManager.Instance.OnDrag(eventData.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (InventoryDragManager.Instance != null)
        {
            InventoryDragManager.Instance.EndDrag();
        }
    }

    #endregion
}