using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    // 다른 스크립트에서 읽을 수 있도록 프로퍼티로 노출
    public Vector2 MoveInput { get; private set; }
    public bool InteractTriggered { get; private set; } // 상호작용 트리거 추가
    public bool AttackTriggered { get; private set; } // 나중에 공격 구현 시 사용

    // Player Input 컴포넌트에서 Send Messages 방식으로 호출
    public void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();
    }

    public void OnInteract(InputValue value)
    {
        InteractTriggered = value.isPressed;
    }

    public void OnAttack(InputValue value)
    {
        // 버튼을 누른 순간만 감지
        AttackTriggered = value.isPressed;
    }

    // 로직 처리 후 트리거를 꺼주기 위한 메서드
    public void ResetInteractTrigger() => InteractTriggered = false;
    // 로직 처리가 끝난 후 트리거를 초기화해야 할 경우 사용
    public void ResetAttackTrigger() => AttackTriggered = false;
}
