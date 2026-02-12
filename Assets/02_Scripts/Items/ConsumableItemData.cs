using UnityEngine;

/// <summary>소모품 베이스. HealItemData, BuffItemData 등으로 구체화.</summary>
public abstract class ConsumableItemData : ItemData
{
    public override ItemType ItemType => ItemType.Consumable;

    /// <summary>소모품 사용 시 효과를 target에 적용.</summary>
    public abstract void ApplyTo(IItemUser target);
}
