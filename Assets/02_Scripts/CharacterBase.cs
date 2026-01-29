using UnityEngine;
using UnityEngine.AI;

public class CharacterBase : MonoBehaviour
{
    public CharacterControlState _state = CharacterControlState.AI;
    private NavMeshAgent _agent;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void SetControlState(CharacterControlState newState, Transform leader = null)
    {
        _state = newState;

        if (_state == CharacterControlState.Player)
        {
            _agent.isStopped = true; // 플레이어일 땐 길찾기 중지
            _agent.enabled = false;
            // 나중에 여기에 PlayerInput 스크립트 활성화 추가
        }
        else
        {
            _agent.enabled = true;
            _agent.isStopped = false;
        }
    }

    void Update()
    {
        if (_state == CharacterControlState.AI)
        {
            HandleFollowLogic();
        }
    }

    private void HandleFollowLogic()
    {
        // 리더(플레이어)를 따라가는 로직은 SquadManager에서 대상을 지정해줄 겁니다.
        // 임시로 SquadManager가 목적지를 갱신해준다고 가정합니다.
    }

    public void Move(Vector2 direction)
    {
        if (_state != CharacterControlState.Player) return;

        // 1. 카메라 방향 데이터 가져오기
        Transform camTransform = Camera.main.transform;
        Vector3 forward = camTransform.forward;
        Vector3 right = camTransform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // 2. 실제 이동 방향 벡터 계산
        Vector3 moveDir = (forward * direction.y) + (right * direction.x);

        // 3. 입력이 있을 때만 로직 수행 (이게 중요!)
        if (moveDir.magnitude > 0.1f)
        {
            // 이동: Translate보다는 이 방식이 물리나 충돌 계산 시 더 안정적입니다.
            transform.position += moveDir * Time.deltaTime * 5f;

            // 회전: Slerp의 세 번째 인자(속도)를 조절해 부드러움을 맞춥니다.
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15f);
        }
    }

    public void SetDestination(Vector3 targetPos)
    {
        if (_agent.enabled && _state == CharacterControlState.AI)
        {
            _agent.SetDestination(targetPos);
        }
    }
}

public enum CharacterControlState { Player, AI }