using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    // 1. 연속적인 입력 (변수 방식 유지)
    public Vector2 MoveInput { get; private set; }

    // 2. 단발성 입력 (이벤트 방식)
    // 누군가 이 이벤트를 구독하면, 키가 눌릴 때마다 소식을 들을 수 있습니다.
    public event Action OnInteractPerformed;
    public event Action OnAttackPerformed;
    public event Action OnInventoryPerformed;

    // Player Input 컴포넌트에서 Send Messages 방식으로 호출됩니다.

    public void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();
    }

    public void OnInteract(InputValue value)
    {
        // 키를 누른 순간(isPressed가 true일 때) 이벤트를 쏩니다.
        if (value.isPressed)
        {
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
        {
            OnInventoryPerformed?.Invoke();
        }
    }
}