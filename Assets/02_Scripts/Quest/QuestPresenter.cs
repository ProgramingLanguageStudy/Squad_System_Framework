using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Model과 View를 연결 (MVP의 Presenter). Model·View는 인스펙터에서 할당.
/// Model 상태 변경 시 데이터를 가져와 표시용으로 변환한 뒤 View에 전달합니다.
/// </summary>
public class QuestPresenter : MonoBehaviour
{
    [SerializeField] private QuestModel _model;
    [SerializeField] private QuestView _view;

    private void Awake()
    {
        if (_model == null)
            Debug.LogWarning($"[QuestPresenter] {gameObject.name}: Model(QuestModel)이 할당되지 않았습니다. 인스펙터에서 할당해 주세요.");
        if (_view == null)
            Debug.LogWarning($"[QuestPresenter] {gameObject.name}: View(QuestView)가 할당되지 않았습니다. 인스펙터에서 할당해 주세요.");
    }

    private void OnEnable()
    {
        if (_model != null)
            _model.OnQuestUpdated += RefreshView;
        if (_view != null)
            _view.SetPanelActive(true);
        RefreshView(null);
    }

    private void OnDisable()
    {
        if (_model != null)
            _model.OnQuestUpdated -= RefreshView;
    }

    private void RefreshView(ActiveQuest _)
    {
        if (_view == null) return;
        if (_model == null)
        {
            _view.SetDisplayText("");
            return;
        }

        var quests = _model.GetActiveQuests();
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
        _view.SetDisplayText(sb.Length > 0 ? sb.ToString() : "진행 중인 퀘스트가 없습니다.");
    }
}
