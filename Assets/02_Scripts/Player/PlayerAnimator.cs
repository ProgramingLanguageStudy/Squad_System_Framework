using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator _animator;
    private PlayerMover _mover; // 속도 값을 가져오기 위해 참조

    private static readonly int SpeedHash = Animator.StringToHash("MoveSpeed");

    public void Initialize(PlayerMover mover)
    {
        _animator = GetComponent<Animator>();
        _mover = mover;
    }

    private void Update()
    {
        // Mover에게 물어봐서 속도 값을 받아옵니다.
        float speed = _mover.GetCurrentSpeed();

        // 블렌드 트리 파라미터 업데이트
        _animator.SetFloat(SpeedHash, speed, 0.1f, Time.deltaTime);
    }
}