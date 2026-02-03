using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string ItemId;     // ������ ���� ID (���� �����)
    public string ItemName;   // �̸�
    public Sprite Icon;       // �κ��丮�� ǥ�õ� �̹���
    public string Description;// ����
    public bool IsStackable;  // ��ġ�� ���� ����
    public int MaxStack = 50; // �⺻�� 50�� (�����۸��� �ٸ��� ���� ����)
}