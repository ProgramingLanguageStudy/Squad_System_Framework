using UnityEngine;

/// <summary>
/// DialogueModel과 DialogueView 연결. View 입력 시 System 제어만 호출.
/// </summary>
public class DialoguePresenter : MonoBehaviour
{
    [SerializeField] private DialogueSystem _system;
    [SerializeField] private DialogueView _view;

    private DialogueModel Model => _system != null ? _system.Model : null;

    private void Awake()
    {
        if (_system == null) _system = FindFirstObjectByType<DialogueSystem>();
        if (_view == null) _view = FindFirstObjectByType<DialogueView>();
        if (_system == null) Debug.LogWarning("[DialoguePresenter] DialogueSystem이 없습니다.");
        if (_view == null) Debug.LogWarning("[DialoguePresenter] DialogueView가 없습니다.");
    }

    private void OnEnable()
    {
        if (Model != null)
            Model.OnDialogueStateChanged += RefreshView;
        if (_view != null)
        {
            _view.OnNextClicked += HandleNext;
            _view.OnEndClicked += HandleEnd;
        }
    }

    private void OnDisable()
    {
        if (Model != null)
            Model.OnDialogueStateChanged -= RefreshView;
        if (_view != null)
        {
            _view.OnNextClicked -= HandleNext;
            _view.OnEndClicked -= HandleEnd;
        }
    }

    private void RefreshView()
    {
        if (_view == null) return;
        if (Model != null && Model.IsTalking)
            _view.Display(Model.GetSpeakerName(), Model.GetCurrentSentence());
        else
            _view.Close();
    }

    private void HandleNext()
    {
        if (_system == null) return;
        if (_view != null && _view.TrySkipTyping())
            return;
        _system.DisplayNextSentence();
    }

    private void HandleEnd()
    {
        _system?.EndDialogue();
    }

    public void SetQuestButtonVisible(bool visible, string buttonText = "퀘스트")
    {
        if (_view != null)
            _view.SetQuestButtonVisible(visible, buttonText);
    }
}
