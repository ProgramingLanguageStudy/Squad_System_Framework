using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>QuestSystem(Model)과 QuestView(View) 연결만 담당. OnQuestUpdated → RefreshView.</summary>
public class QuestPresenter : MonoBehaviour
{
    [SerializeField] private QuestSystem _system;
    [SerializeField] private QuestView _view;

    /// <summary>PlayScene에서 QuestController·SaveCoordinator 등에 주입용.</summary>
    public QuestSystem System => _system;

    private void Awake()
    {
        if (_system == null)
            Debug.LogWarning($"[QuestPresenter] {gameObject.name}: QuestSystem이 할당되지 않았습니다. 인스펙터에서 할당해 주세요.");
        if (_view == null)
            Debug.LogWarning($"[QuestPresenter] {gameObject.name}: View(QuestView)가 할당되지 않았습니다. 인스펙터에서 할당해 주세요.");
    }

    private void OnEnable()
    {
        if (_system != null)
            _system.OnQuestUpdated += RefreshView;
        if (_view != null)
            _view.SetPanelActive(true);
        RefreshView(null);
    }

    private void OnDisable()
    {
        if (_system != null)
            _system.OnQuestUpdated -= RefreshView;
    }

    private void RefreshView(QuestModel _)
    {
        if (_view == null) return;
        if (_system == null)
        {
            _view.SetDisplayText("");
            return;
        }

        IReadOnlyList<QuestModel> quests = _system.GetActiveQuests();
        var sb = new StringBuilder();
        foreach (var quest in quests)
        {
            sb.AppendLine($"<b>{quest.Title}</b>");
            string progress = $"{quest.CurrentAmount}/{quest.TargetAmount}";
            string done = quest.IsCompleted ? " [완료]" : "";
            sb.AppendLine($"  - {quest.Description} ({progress}){done}");
            sb.AppendLine();
        }
        _view.SetDisplayText(sb.Length > 0 ? sb.ToString() : "진행 중인 퀘스트가 없습니다.");
    }
}
