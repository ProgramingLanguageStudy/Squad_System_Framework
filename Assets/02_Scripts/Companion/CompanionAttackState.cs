using UnityEngine;

public class CompanionAttackState : CompanionStateBase
{
    private float _timer;

    public CompanionAttackState(CompanionStateMachine machine) : base(machine) { }

    public override void Enter()
    {
        _timer = 0f;

        // 1. 공격 시작 시점에 딱 한 번만 타겟을 향해 방향 정렬 (보정)
        FaceTargetImmediate();

        // 2. 캐릭터 컴포넌트 정지 및 애니메이션 실행
        var character = Machine.Character;

        // Mover를 즉시 멈춤 (NavMeshAgent 관성 제거)
        var mover = character?.GetComponent<CharacterFollowMover>();
        mover?.Stop();

        character?.Animator?.Attack();
        character?.Attacker?.OnAttackStarted();
    }

    public override void Update()
    {
        _timer += Time.deltaTime;

        // Update에서는 아무것도 하지 않음 (이동/회전 금지 약속 준수)
        if (_timer >= Machine.AttackDuration)
        {
            Machine.RequestCombat();
        }
    }

    private void FaceTargetImmediate()
    {
        // CombatController를 통해 현재 가장 가까운 적을 찾음
        var enemy = Machine.CombatController?.GetNearestEnemy(Machine.transform.position);
        if (enemy == null) return;

        Vector3 dir = (enemy.transform.position - Machine.transform.position).normalized;
        dir.y = 0; // 바닥 평면 기준으로만 회전

        if (dir.sqrMagnitude > 0.01f)
        {
            // 공격 시작 프레임에 즉시 적을 바라보게 함
            Machine.transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    public override void Exit()
    {
        Machine.Character?.Attacker?.EndAttackCleanup();
    }

    // 상태 머신의 약속: 공격 중 이동/회전 불가
}