using UnityEngine;

/// <summary>
/// 세이브 파일 내 플레이어 섹션. 위치·회전·체력 등. 나중에 필드 추가.
/// </summary>
[System.Serializable]
public class PlayerSaveData
{
    /// <summary>마지막 조종 캐릭터 식별. CharacterData.characterId 또는 displayName</summary>
    public string playerCharacterId = "";
    public Vector3 position;
    public float rotationY;
    public int currentHp;
}
