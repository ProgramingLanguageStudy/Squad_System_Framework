using UnityEngine;
using TMPro;

/// <summary>
/// 퀘스트 트래커 UI 표시만 담당 (MVP의 View).
/// 받은 문자열을 그대로 그리며, Model·이벤트를 알지 못합니다.
/// </summary>
public class QuestView : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _logText;

    /// <summary>표시할 텍스트를 설정합니다. Presenter에서만 호출합니다.</summary>
    public void SetDisplayText(string text)
    {
        if (_logText != null)
            _logText.text = text ?? "";
    }

    /// <summary>패널 활성/비활성. 트래커는 항상 표시할 경우 Awake 등에서 true.</summary>
    public void SetPanelActive(bool active)
    {
        if (_panel != null)
            _panel.SetActive(active);
    }

    /// <summary>트래커는 항상 표시되므로 이동 제한에 사용하지 않음.</summary>
    public bool IsOpen => false;
}
