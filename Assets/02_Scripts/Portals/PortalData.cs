using UnityEngine;

/// <summary>
/// 포탈 표시용 데이터. 순간이동 목록 UI 등에서 이름/아이콘 표시에 사용.
/// </summary>
[CreateAssetMenu(fileName = "PortalData", menuName = "Portal/Data")]
public class PortalData : ScriptableObject
{
    [Tooltip("순간이동 목록 등에 표시할 이름 (예: 마을, 던전 입구)")]
    public string displayName = "알 수 없음";

    [Tooltip("선택) 툴팁이나 설명에 쓸 문구")]
    [TextArea(1, 2)]
    public string description;

    [Tooltip("선택) 목록 UI에 쓸 아이콘")]
    public Sprite icon;
}
