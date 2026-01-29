using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] private float _rotationSpeed = 15f;

    private NavMeshAgent _agent;
    private InputHandler _input;
    private Transform _mainCameraTransform; // 카메라 위치 정보 저장용
    private Animator _animator;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _input = GetComponent<InputHandler>();
        _animator = GetComponent<Animator>();

        // 메인 카메라의 트랜스폼을 미리 찾아둡니다.
        _mainCameraTransform = Camera.main.transform;

        _agent.updateRotation = false;
    }

    private void Update()
    {
        MovePlayer();
        UpdateAnimation();
    }

    private void MovePlayer()
    {
        Vector2 input = _input.MoveInput;
        if (input.magnitude < 0.1f)
        {
            _agent.velocity = Vector3.zero;
            return;
        }

        // 플레이어가 바라보는 방향(=카메라의 방향)의 forward 벡터와 right 벡터 가져오기
        Vector3 forward = _mainCameraTransform.forward;
        Vector3 right = _mainCameraTransform.right;

        // 가져온 두 벡터의 y값을 0으로 맞춘 후 Normalize -> 카메라는 실제로 수평을 바라보는게 아닌 살짝 위에서 아래를 보고있기 때문에
        // 위 벡터는 단순 좌표평면상의 앞뒤와 좌우라는 "방향"만 제시하기 위함. 
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // 3. 입력값(WASD)과 카메라 방향을 조합해 최종 이동 방향 계산
        // 이 moveDir은 실제 캐릭터가 입력받았을 때 나아갈 '방향'을 의미함
        Vector3 moveDir = (forward * input.y) + (right * input.x);
        moveDir.Normalize();

        // --- 이동속도 및 회전속도 적용 ---

        if (moveDir.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            // 위에서 최종계산할 나아갈 '방향'에 이동속도 곱하기
            _agent.velocity = moveDir * _moveSpeed;
        }
    }

    private void UpdateAnimation()
    {
        // 입력이 없을 때는 에이전트의 속도와 상관없이 강제로 0으로 판단
        float targetSpeed = (_input.MoveInput.magnitude < 0.1f) ? 0f : _agent.velocity.magnitude;

        // 만약 여전히 애니메이션이 늦게 멈춘다면 0.1f(Damp Time)를 0.05f로 줄이거나 0으로 바꿔보세요.
        _animator.SetFloat("MoveSpeed", targetSpeed, 0.05f, Time.deltaTime);
    }
}
