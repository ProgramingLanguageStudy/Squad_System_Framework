using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem; // 필수 추가!

public class SquadManager : MonoBehaviour
{
    [SerializeField] private List<CharacterBase> _squadMembers;
    [SerializeField] private CinemachineCamera _vCam; // 인스펙터에서 할당
    private CharacterBase _currentLeader;

    // New Input System 관련 변수
    private PlayerInputActions _inputActions;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _inputActions.Squad.Enable();

        // SwitchCharacter 액션이 수행되었을 때 실행
        _inputActions.Squad.SwitchCharacter.performed += OnSwitchInput;
    }

    private void OnDisable()
    {
        _inputActions.Squad.SwitchCharacter.performed -= OnSwitchInput;
        _inputActions.Squad.Disable();
    }

    private void OnSwitchInput(InputAction.CallbackContext context)
    {
        // 눌린 키의 바인딩 정보나 컨트롤 이름을 가져옵니다.
        // "1", "2", "3" 키 이름을 직접 체크하거나 컨트롤 경로를 활용할 수 있습니다.
        string controlName = context.control.name; // 예: "1", "2", "3"

        if (int.TryParse(controlName, out int number))
        {
            // 입력은 1, 2, 3이지만 리스트 인덱스는 0, 1, 2이므로 -1을 해줍니다.
            SwitchLeader(number - 1);
        }
    }

    private void Start()
    {
        if (_squadMembers.Count > 0)
            SwitchLeader(0);
    }

    private void Update()
    {
        {
            // WASD 입력에 따라 (x: -1~1, y: -1~1) 값이 들어옵니다.
            Vector2 moveInput = _inputActions.Squad.Move.ReadValue<Vector2>();

            if (_currentLeader != null && moveInput != Vector2.zero)
            {
                _currentLeader.Move(moveInput);
            }

            UpdateAIOrders();
        }
    }

    private void SwitchLeader(int index)
{
    if (index < 0 || index >= _squadMembers.Count) return;

    _currentLeader = _squadMembers[index];

    for (int i = 0; i < _squadMembers.Count; i++)
    {
        if (_squadMembers[i] == _currentLeader)
        {
            _squadMembers[i].SetControlState(CharacterControlState.Player);
            // 리더가 된 캐릭터는 이전 AI 목적지를 초기화 (그 자리에서 멈춤)
            _squadMembers[i].SetDestination(_squadMembers[i].transform.position);
        }
        else
        {
            _squadMembers[i].SetControlState(CharacterControlState.AI);
        }
    }

    UpdateCameraTarget();
}

    private void UpdateCameraTarget()
    {
        if (_currentLeader == null || _vCam == null) return;

        // 시네머신 카메라가 새로운 리더를 따라다니고 바라보게 설정
        _vCam.Follow = _currentLeader.transform;
        _vCam.LookAt = _currentLeader.transform;
    }

    private void UpdateAIOrders()
    {
        if (_currentLeader == null) return;

        // 리더를 제외한 나머지 AI 멤버들의 순번을 따로 매깁니다.
        int aiIndex = 0;
        int aiCount = _squadMembers.Count - 1;

        for (int i = 0; i < _squadMembers.Count; i++)
        {
            var member = _squadMembers[i];
            if (member == _currentLeader) continue;

            float distance = Vector3.Distance(member.transform.position, _currentLeader.transform.position);

            // 1. 추적 시작 거리 (충분히 멀어지면 목적지 갱신)
            if (distance > 4.0f)
            {
                // AI 멤버들끼리만 360도를 나눠 가집니다.
                float angle = aiIndex * (360f / aiCount);
                float rad = angle * Mathf.Deg2Rad; // 각도를 라디안으로 변환

                // 원형 배치 좌표 계산
                Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * 2.5f;

                member.SetDestination(_currentLeader.transform.position + offset);
            }
            // 2. 멈춤 거리 (리더 주변에 도착하면 내비게이션 중단)
            else if (distance < 2.0f)
            {
                // 목적지를 자기 자신의 현재 위치로 설정하여 그 자리에 멈추게 함
                member.SetDestination(member.transform.position);
            }

            aiIndex++; // 다음 AI 멤버를 위해 순번 증가
        }
    }
}