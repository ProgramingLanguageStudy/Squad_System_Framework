using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI countText;

    // 슬롯 비우기 (초기 상태)
    public void ClearSlot()
    {
        icon.sprite = null;
        icon.enabled = false;
        countText.text = "";
    }

    // 아이템 정보 채우기
    public void UpdateSlot(ItemData item, int count)
    {
        if (item == null || count <= 0)
        {
            ClearSlot();
            return;
        }

        icon.sprite = item.Icon;
        icon.enabled = true;
        countText.text = count > 1 ? count.ToString() : "";
    }
}