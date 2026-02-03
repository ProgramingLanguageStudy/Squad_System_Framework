using UnityEngine;

[RequireComponent(typeof(PlayerMover)), RequireComponent(typeof(PlayerAnimator)), RequireComponent(typeof(PlayerInteractor))]
public class Player : MonoBehaviour
{
    // 각 부품들
    public InputHandler InputHandler { get; private set; }
    public PlayerMover Mover { get; private set; }
    public PlayerAnimator Animator { get; private set; }
    public PlayerInteractor Interactor { get; private set; }

    public bool CanMove { get; set; } = true;

    public void Initialize()
    {
        // 내부 컴포넌트 수집 (Awake에서 해도 되지만 여기서 일괄 관리 가능)
        InputHandler = GetComponent<InputHandler>();
        Mover = GetComponent<PlayerMover>();
        Animator = GetComponent<PlayerAnimator>();
        Interactor = GetComponent<PlayerInteractor>();

        // 부품들 연결
        Mover.Initialize();
        Animator.Initialize(Mover);
        Interactor.Initialize(this);
    }

    private void Update()
    {
        // 1. CanMove가 false면 이동 입력을 완전히 무시
        if (!CanMove)
        {
            Mover.Move(Vector2.zero); // 즉시 정지
            return;
        }

        // 2. 평상시 이동
        Mover.Move(InputHandler.MoveInput);
    }
}