using UnityEngine;

/// <summary>
/// 세이브 파일 루트. 섹션별 데이터를 묶어서 한 번에 JSON 직렬화.
/// </summary>
[System.Serializable]
public class SaveData
{
    public PlayerSaveData player = new PlayerSaveData();
}
