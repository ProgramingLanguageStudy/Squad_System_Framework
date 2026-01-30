using UnityEngine;

[RequireComponent (typeof(InputHandler)), RequireComponent(typeof(PlayerMover)), RequireComponent(typeof(PlayerAnimator)), RequireComponent(typeof(PlayerInteractor))]
public class Player : MonoBehaviour
{
    // 각 부품들
    public InputHandler InputHandler { get; private set; }
    public PlayerMover Mover { get; private set; }
    public PlayerAnimator Animator { get; private set; }
    public PlayerInteractor Interactor { get; private set; }

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
        Interactor.Initialize(this, InputHandler);
    }

    private void Update()
    {
        // InputHandler에서 받은 입력값을 전달하여 Player 움직이기
        Mover.Move(InputHandler.MoveInput);
    }
}