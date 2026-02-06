using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 대화 진입점. 데이터 로드·선택·제어(시작/다음/종료). Model은 상태만, System이 제어. 씬에 한 개 두거나 조율층에서 주입.
/// </summary>
public class DialogueSystem : MonoBehaviour
{
    private readonly DialogueModel _model = new DialogueModel();
    private Action _onComplete;

    private Dictionary<string, DialogueData> _dialogueById = new Dictionary<string, DialogueData>();
    private Dictionary<string, List<DialogueData>> _dialogueByNpcId = new Dictionary<string, List<DialogueData>>();

    public bool IsLoaded { get; private set; } = false;
    public DialogueModel Model => _model;
    public bool IsTalking => _model.IsTalking;
    public string CurrentNpcId => _model.CurrentNpcId;
    public string CurrentSpeakerName => _model.CurrentSpeakerName;
    public DialogueType CurrentDialogueType => _model.CurrentDialogueType;

    public event Action OnDialogueEnd
    {
        add => _model.OnDialogueEnd += value;
        remove => _model.OnDialogueEnd -= value;
    }

    private void Awake()
    {
        StartCoroutine(LoadNextFrame());
    }

    private void OnEnable()
    {
        GameEvents.OnPlayDialogueRequested += HandlePlayDialogueRequested;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayDialogueRequested -= HandlePlayDialogueRequested;
    }

    /// <summary>연결 포트. 외부(Interactor 등)가 OnPlayDialogueRequested 발행 시 여기서 재생.</summary>
    private void HandlePlayDialogueRequested(DialogueData data)
    {
        if (data != null && IsLoaded && !IsTalking)
            StartDialogue(data);
    }

    private IEnumerator LoadNextFrame()
    {
        yield return null;
        LoadAll();
    }

    private void LoadAll()
    {
        var assets = Resources.LoadAll<DialogueData>("Dialogues");
        _dialogueById.Clear();
        _dialogueByNpcId.Clear();
        if (assets != null)
        {
            for (int i = 0; i < assets.Length; i++)
            {
                var d = assets[i];
                if (d == null) continue;
                if (!string.IsNullOrEmpty(d.id))
                    _dialogueById[d.id] = d;
                if (!string.IsNullOrEmpty(d.npcId))
                {
                    if (!_dialogueByNpcId.TryGetValue(d.npcId, out var list))
                    {
                        list = new List<DialogueData>();
                        _dialogueByNpcId[d.npcId] = list;
                    }
                    list.Add(d);
                }
            }
        }
        IsLoaded = true;
        Debug.Log($"<color=cyan>[DialogueSystem]</color> 대화 {_dialogueById.Count}개 로드.");
    }

    /// <summary>FirstMeet 우선, 없으면 Common 중 랜덤, 없으면 첫 번째.</summary>
    public DialogueData GetBestDialogue(string npcId)
    {
        if (!IsLoaded || !_dialogueByNpcId.TryGetValue(npcId, out var list) || list.Count == 0)
        {
            if (IsLoaded) Debug.LogWarning($"[DialogueSystem] {npcId}에 해당하는 대화가 없습니다.");
            return null;
        }
        for (int i = 0; i < list.Count; i++)
            if (list[i].dialogueType == DialogueType.FirstMeet) return list[i];
        int commonCount = 0;
        for (int i = 0; i < list.Count; i++)
            if (list[i].dialogueType == DialogueType.Common) commonCount++;
        if (commonCount > 0)
        {
            int pick = UnityEngine.Random.Range(0, commonCount);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].dialogueType != DialogueType.Common) continue;
                if (pick-- == 0) return list[i];
            }
        }
        return list[0];
    }

    public DialogueData GetDialogueById(string id)
    {
        return IsLoaded && _dialogueById.TryGetValue(id, out var data) ? data : null;
    }

    /// <summary>대화 시작. System이 Model에 데이터 세팅.</summary>
    public void StartDialogue(DialogueData data, Action onComplete = null)
    {
        if (data == null) return;
        _onComplete = onComplete;
        _model.SetDialogue(data);
    }

    /// <summary>다음 문장으로. System이 인덱스 증가 또는 종료 판단.</summary>
    public void DisplayNextSentence()
    {
        if (!_model.IsTalking) return;
        int next = _model.CurrentIndex + 1;
        if (next >= _model.LineCount)
        {
            _model.Clear();
            _onComplete?.Invoke();
            _onComplete = null;
        }
        else
        {
            _model.SetCurrentIndex(next);
        }
    }

    /// <summary>대화 강제 종료.</summary>
    public void EndDialogue()
    {
        if (!_model.IsTalking) return;
        _model.Clear();
        _onComplete?.Invoke();
        _onComplete = null;
    }

    public void SetQuestButtonVisible(bool visible)
    {
        var presenter = FindFirstObjectByType<DialoguePresenter>();
        if (presenter != null) presenter.SetQuestButtonVisible(visible);
    }
}
