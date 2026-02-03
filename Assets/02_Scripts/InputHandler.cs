using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    // 1. 이동 입력 (매 프레임 갱신)
    public Vector2 MoveInput { get; private set; }

    // 2. 액션 입력 (이벤트 방식)
    // 버튼을 누르면 이벤트가 발생하고, 키를 뗐을 때도 이벤트가 발생할 수 있습니다.
    public event Action OnInteractPerformed;
    public event Action OnAttackPerformed;
    public event Action OnInventoryPerformed;
    public event Action OnQuestPerformed;

    // Player Input 컴포넌트에서 Send Messages 방식으로 호출됩니다.

    public void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();
    }

    public void OnInteract(InputValue value)
    {
        // 키가 눌린 순간(isPressed가 true일 때) 이벤트를 발생시킵니다.
        if (value.isPressed)
        {
            Debug.Log("상호작용 입력");
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
