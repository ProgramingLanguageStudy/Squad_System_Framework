using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

/// <summary>
/// 진행 중인 퀘스트 목록을 표시. OnQuestUpdated 시 자동 갱신.
/// 패널/텍스트는 인스펙터에서 연결. 열기/닫기는 Toggle() 또는 패널 SetActive로 제어.
/// </summary>
public class QuestUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _logText;

    private void OnEnable()
    {
        GameEvents.OnQuestUpdated += OnQuestUpdated;
        Refresh();
    }

    private void OnDisable()
    {
        GameEvents.OnQuestUpdated -= OnQuestUpdated;
    }

    private void OnQuestUpdated(QuestData _)
    {
        Refresh();
    }

    public void Toggle()
    {
        if (_panel != null)
            _panel.SetActive(!_panel.activeSelf);
    }

    /// <summary>패널이 열려 있는지 (이동 제한 등에 사용).</summary>
    public bool IsOpen => _panel != null && _panel.activeSelf;

    public void Refresh()
    {
        if (_logText == null) return;
        if (QuestManager.Instance == null)
        {
            _logText.text = "";
            return;
        }

        var quests = QuestManager.Instance.GetActiveQuests();
        var sb = new StringBuilder();

        foreach (var quest in quests)
        {
            sb.AppendLine($"<b>{quest.Title}</b>");
            foreach (var task in quest.Tasks)
            {
                string progress = $"{task.CurrentAmount}/{task.TargetAmount}";
                string done = task.IsCompleted ? " [완료]" : "";
                sb.AppendLine($"  · {task.Description} ({progress}){done}");
            }
            sb.AppendLine();
        }

        _logText.text = sb.Length > 0 ? sb.ToString() : "진행 중인 퀘스트가 없습니다.";
    }
}
