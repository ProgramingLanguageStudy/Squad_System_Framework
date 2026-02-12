using UnityEngine;

[CreateAssetMenu(fileName = "New Buff Item", menuName = "Inventory/Buff Item")]
public class BuffItemData : ConsumableItemData
{
    [Tooltip("일시 적용될 스탯 보정")]
    public PlayerStatModifier Modifier;
    [Tooltip("지속 시간(초)")]
    public float DurationSeconds = 30f;

    public override void ApplyTo(IItemUser target)
    {
        if (target != null && DurationSeconds > 0f)
            target.ApplyBuff(Modifier, DurationSeconds);
    }
}
