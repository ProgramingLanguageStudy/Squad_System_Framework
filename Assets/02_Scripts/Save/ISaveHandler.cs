/// <summary>
/// 세이브/로드에 참여하는 시스템이 구현. DataManager에 등록 후 Gather 시 자기 섹션만 채우고, Apply 시 자기 섹션만 적용.
/// </summary>
public interface ISaveHandler
{
    void Gather(SaveData data);
    void Apply(SaveData data);
}
