using UnityEngine;

/// <summary>
/// 카메라 기준 이동 방향 계산 + CharacterController.Move로 이동·회전.
/// NavMesh 미사용. Npc·몬스터 등 일반 Collider와 충돌해 통과하지 않음.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 15f;
    [SerializeField] [Tooltip("비이동 시 적용할 중력 배율 (지면 고정용)")]
    private float _gravity = 20f;

    private CharacterController _controller;
    private Transform _mainCameraTransform;
    private PlayerModel _model;
    private float _lastSpeed;

    public void Initialize(CharacterController controller, Transform mainCameraTransform, PlayerModel model)
    {
        _controller = controller;
        _mainCameraTransform = mainCameraTransform;
        _model = model;
    }

    /// <summary>
    /// Player의 Update에서 호출해서 이동을 처리하는 함수
    /// </summary>
    public void Move(Vector2 input)
    {
        float speed = _model != null ? _model.MoveSpeed : 6f;
        Vector3 motion;

        if (input.magnitude < 0.1f)
        {
            _lastSpeed = 0f;
            motion = Vector3.down * (_gravity * Time.deltaTime);
            _controller.Move(motion);
            return;
        }

        Vector3 forward = _mainCameraTransform.forward;
        Vector3 right = _mainCameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = (forward * input.y) + (right * input.x);
        moveDir.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        motion = moveDir * (speed * Time.deltaTime);
        motion += Vector3.down * (_gravity * Time.deltaTime);
        _controller.Move(motion);
        _lastSpeed = speed;
    }

    /// <summary>Animator에 속도를 넘겨줄 때 쓸 값의 Getter 용도</summary>
    public float GetCurrentSpeed() => _lastSpeed;
}
