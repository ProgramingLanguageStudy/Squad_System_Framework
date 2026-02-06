using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string ItemId;     // 인벤토리 내부 ID (저장/로드용)
    public string ItemName;   // 이름
    public Sprite Icon;       // 인벤토리에 표시할 이미지
    public string Description;// 설명
    public bool IsStackable;  // 겹침 가능 여부
    public int MaxStack = 50; // 기본 50개 (아이템별로 다르게 두려면 에셋에서 수정)
}
