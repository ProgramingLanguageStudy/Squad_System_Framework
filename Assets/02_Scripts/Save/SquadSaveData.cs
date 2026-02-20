using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 분대 전체 저장 데이터. 플레이어 위치 + 조종 캐릭터 ID + 멤버별 ID·체력.
/// 동료 위치는 저장하지 않음. 로드 시 플레이어 주위로 재배치.
/// </summary>
[System.Serializable]
public class SquadSaveData
{
    public string currentPlayerId = "";
    public Vector3 playerPosition;
    public float playerRotationY;
    public List<CharacterMemberData> members = new List<CharacterMemberData>();
}
