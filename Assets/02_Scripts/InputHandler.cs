using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    // 1. �������� �Է� (���� ��� ����)
    public Vector2 MoveInput { get; private set; }

    // 2. �ܹ߼� �Է� (�̺�Ʈ ���)
    // ������ �� �̺�Ʈ�� �����ϸ�, Ű�� ���� ������ �ҽ��� ���� �� �ֽ��ϴ�.
    public event Action OnInteractPerformed;
    public event Action OnAttackPerformed;
    public event Action OnInventoryPerformed;
    public event Action OnQuestPerformed;

    // Player Input ������Ʈ���� Send Messages ������� ȣ��˴ϴ�.

    public void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();
    }

    public void OnInteract(InputValue value)
    {
        // Ű�� ���� ����(isPressed�� true�� ��) �̺�Ʈ�� ���ϴ�.
        if (value.isPressed)
        {
            Debug.Log("��ȣ�ۿ� ����");
            OnInteractPerformed?.Invoke();
        }
    }

    public void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            OnAttackPerformed?.Invoke();
        }
    }

    public void OnInventory(InputValue value)
    {
        if (value.isPressed)
            OnInventoryPerformed?.Invoke();
    }

    public void OnQuest(InputValue value)
    {
        if (value.isPressed)
            OnQuestPerformed?.Invoke();
    }
}