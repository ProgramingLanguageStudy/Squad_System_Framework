using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

/// <summary>
/// 퀘스트 트래커 UI. 진행 중인 퀘스트를 항상 표시하며, OnQuestUpdated 시 자동 갱신.
/// 패널/텍스트는 인스펙터에서 연결. (전체 퀘스트 목록은 별도 UI에서 Q 키로 열 예정)
/// </summary>
public class QuestUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _logText;

    private void OnEnable()
    {
        if (_panel != null)
            _panel.SetActive(true); // 트래커는 항상 표시
        GameEvents.OnQuestUpdated += OnQuestUpdated;
        StartCoroutine(RefreshNextFrame());
    }

    private IEnumerator RefreshNextFrame()
    {
        yield return null;
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

    /// <summary>전체 퀘스트 목록용으로 나중에 사용. 현재 트래커 패널은 항상 표시.</summary>
    public void Toggle()
    {
        // 트래커는 항상 켜 둠. 나중에 전체 목록 패널 토글 시 여기서 처리 가능
    }

    /// <summary>트래커는 항상 표시되므로 이동 제한에 사용하지 않음.</summary>
    public bool IsOpen => false;

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
                sb.AppendLine($"  - {task.Description} ({progress}){done}");
            }
            sb.AppendLine();
        }

        _logText.text = sb.Length > 0 ? sb.ToString() : "진행 중인 퀘스트가 없습니다.";
    }
}
