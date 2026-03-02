/// <summary>
/// 이동 컴포넌트 공통 계약.
/// Character는 이 인터페이스만 알면 되고, 구현 방식(CC / NavMesh)은 몰라도 됩니다.
/// </summary>
public interface IMover
{
    /// <summary>
    /// Model의 현재 속도 설정함수
    /// </summary>
    /// <param name="speed"></param>
    void SetCurrentMoveSpeed(float speed);

    /// <summary>즉시 정지. 경로/속도 모두 초기화.</summary>
    void Stop();
}