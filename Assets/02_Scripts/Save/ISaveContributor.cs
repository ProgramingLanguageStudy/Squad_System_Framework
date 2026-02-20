/// <summary>
/// 세이브/로드에 기여하는 도메인. PlaySaveCoordinator가 수집해 Gather/Apply 시 위임.
/// SaveOrder가 작을수록 먼저 실행. 같은 값은 순서 무관.
/// </summary>
public interface ISaveContributor
{
    int SaveOrder { get; }
    void Gather(SaveData data);
    void Apply(SaveData data);
}
