using UnityEngine;

/// <summary>
/// 월드 방향을 받아 이동·회전만 담당. 카메라/입력 변환은 호출부에서 처리.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class CharacterMover : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 15f;
    [SerializeField] [Tooltip("비이동 시 적용할 중력 배율")]
    private float _gravity = 20f;

    private CharacterController _controller;
    private CharacterModel _model;
    private float _lastSpeed;

    public void Initialize(CharacterController controller, CharacterModel model)
    {
        _controller = controller;
        _model = model;
    }

    /// <summary>월드 기준 이동 방향. 비어 있으면 중력만 적용. 정규화 여부 무관.</summary>
    public void Move(Vector3 worldDirection)
    {
        if (_controller == null || !_controller.enabled)
        {
            _lastSpeed = 0f;
            return;
        }

        float speed = _model != null ? _model.MoveSpeed : 6f;

        if (worldDirection.sqrMagnitude < 0.01f)
        {
            _lastSpeed = 0f;
            var motion = Vector3.down * (_gravity * Time.deltaTime);
            _controller.Move(motion);
            return;
        }

        Vector3 moveDir = worldDirection;
        moveDir.y = 0f;
        moveDir.Normalize();

        // 이동 방향으로 회전
        Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        var motion2 = moveDir * (speed * Time.deltaTime);
        motion2 += Vector3.down * (_gravity * Time.deltaTime);
        _controller.Move(motion2);
        _lastSpeed = speed;
    }

    public float GetCurrentSpeed() => _lastSpeed;
}
