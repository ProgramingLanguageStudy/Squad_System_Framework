using UnityEngine;

[CreateAssetMenu(fileName = "New Material", menuName = "Inventory/Material Item")]
public class MaterialItemData : ItemData
{
    public override ItemType ItemType => ItemType.Material;
}
