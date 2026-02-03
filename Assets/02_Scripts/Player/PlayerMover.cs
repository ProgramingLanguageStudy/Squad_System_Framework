using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] private float _rotationSpeed = 15f;

    private NavMeshAgent _agent;
    private Transform _mainCameraTransform;

    public void Initialize()
    {
        _agent = GetComponent<NavMeshAgent>();
        _mainCameraTransform = Camera.main.transform;
        _agent.updateRotation = false; // ȸ���� ���� ����
    }

    /// <summary>
    /// Player�� Update���� ȣ���Ͽ� �̵��� �����ϴ� �Լ�
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

        // ȸ�� ó��
        Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        // �̵� ó��
        _agent.velocity = moveDir * _moveSpeed;
    }

    // Animator�� �ӵ��� ������ �� �ֵ��� Getter ����
    public float GetCurrentSpeed() => _agent.velocity.magnitude;
}