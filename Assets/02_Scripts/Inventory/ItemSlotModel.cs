using System;

/// <summary>아이템 슬롯 한 칸의 런타임 데이터 (ItemModel + 개수). Index는 인벤토리 내 위치. MVP의 슬롯 Model.</summary>
[Serializable]
public class ItemSlotModel
{
    public int Index { get; }
    public ItemModel Item;
    public int Count;

    public ItemSlotModel(ItemModel item, int count, int index)
    {
        Item = item;
        Count = count;
        Index = index;
    }

    public void Clear()
    {
        Item = null;
        Count = 0;
    }
}
