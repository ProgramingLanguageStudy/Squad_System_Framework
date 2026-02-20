using UnityEngine;

/// <summary>
/// 분대원 1명의 저장 데이터. ID + 수치(체력 등)만 저장. 위치는 플레이어 기준으로 로드 시 재배치.
/// </summary>
[System.Serializable]
public class CharacterMemberData
{
    public string characterId = "";
    public int currentHp;
}
