using UnityEngine;

/// <summary>
/// 세이브 파일 내 플레이어 섹션. 위치·회전·체력 등. 나중에 필드 추가.
/// </summary>
[System.Serializable]
public class PlayerSaveData
{
    public Vector3 position;
    public float rotationY;
    public int currentHp;
}
