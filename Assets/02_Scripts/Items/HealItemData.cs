using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Item", menuName = "Inventory/Heal Item")]
public class HealItemData : ConsumableItemData
{
    [Tooltip("회복량")]
    public int Amount;

    public override void ApplyTo(IItemUser target)
    {
        if (Amount > 0 && target != null)
            target.Heal(Amount);
    }
}
