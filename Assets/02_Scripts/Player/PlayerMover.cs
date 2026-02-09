using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 15f;

    private NavMeshAgent _agent;
    private Transform _mainCameraTransform;
    private PlayerModel _model;

    public void Initialize(NavMeshAgent agent, Transform mainCameraTransform, PlayerModel model)
    {
        _agent = agent;
        _mainCameraTransform = mainCameraTransform;
        _model = model;
        if (_agent != null)
            _agent.updateRotation = false; // 회전은 직접 제어
    }

    /// <summary>
    /// Player의 Update에서 호출해서 이동을 처리하는 함수
    /// </summary>
    /// <param name="input"></param>
    public void Move(Vector2 input)
    {
        if (input.magnitude < 0.1f)
        {
            _agent.velocity = Vector3.zero;
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

        // 회전 처리
        Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        // 이동 처리 (속도는 PlayerModel.MoveSpeed 사용)
        float speed = _model != null ? _model.MoveSpeed : 6f;
        _agent.velocity = moveDir * speed;
    }

    /// <summary>Animator에 속도를 넘겨줄 때 쓸 값의 Getter 용도</summary>
    public float GetCurrentSpeed() => _agent.velocity.magnitude;
}
