using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment Item")]
public class EquipmentItemData : ItemData
{
    [Tooltip("장비 착용 시 적용되는 스탯 보정")]
    public StatModifier StatModifier;

    public override ItemType ItemType => ItemType.Equipment;
}
