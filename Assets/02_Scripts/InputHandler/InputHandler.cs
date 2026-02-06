using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 생성된 InputSystem_Actions 사용. IPlayerActions를 구현해서 "입력 하나당 메서드 하나"로만 처리.
/// 람다/Subscribe 없이, Input이 OnInteract(), OnAttack() 같은 메서드를 불러주는 방식.
/// </summary>
public class InputHandler : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    private InputSystem_Actions _actions;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    public event Action OnInteractPerformed;
    public event Action OnAttackPerformed;
    public event Action OnInventoryPerformed;
    public event Action OnQuestPerformed;

    private void OnEnable()
    {
        _actions = new InputSystem_Actions();
        _actions.Player.AddCallbacks(this);  // "this의 OnInteract, OnAttack ... 를 입력될 때마다 불러줘"
        _actions.Player.Enable();
    }

    private void Update()
    {
        if (_actions == null) return;
        if (_actions.Player.enabled)
        {
            MoveInput = _actions.Player.Move.ReadValue<Vector2>();
            LookInput = _actions.Player.Look.ReadValue<Vector2>();
        }
    }

    private void OnDisable()
    {
        _actions?.Player.RemoveCallbacks(this);
        _actions?.Player.Disable();
        _actions?.Dispose();
        _actions = null;
    }

    // ----- 아래는 Input이 "이 버튼 눌렸다" 할 때마다 불러주는 메서드들. 그냥 우리 이벤트만 발행하면 됨. -----

    public void OnMove(InputAction.CallbackContext context) { }   // Move는 Update에서 ReadValue로 처리
    public void OnLook(InputAction.CallbackContext context) { }   // Look도 Update에서 처리

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnInteractPerformed?.Invoke();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnAttackPerformed?.Invoke();
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnInventoryPerformed?.Invoke();
    }

    public void OnQuest(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnQuestPerformed?.Invoke();
    }

    public void OnCrouch(InputAction.CallbackContext context) { }
    public void OnJump(InputAction.CallbackContext context) { }
    public void OnPrevious(InputAction.CallbackContext context) { }
    public void OnNext(InputAction.CallbackContext context) { }
    public void OnSprint(InputAction.CallbackContext context) { }
}
