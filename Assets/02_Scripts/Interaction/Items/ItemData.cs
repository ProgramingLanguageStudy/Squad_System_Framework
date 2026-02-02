using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string ItemName;
    public Sprite Icon;
    public string Description;
    public int MaxStack; // 겹칠 수 있는 최대 개수
}