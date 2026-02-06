using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 대화창 표시만 담당 (MVP의 View). Presenter가 Display/Close만 호출. Model·System을 모름.
/// </summary>
public class DialogueView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private GameObject _panel;

    [Header("버튼")]
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _endButton;
    [SerializeField] private Button _questButton;
    [SerializeField] private TextMeshProUGUI _questButtonText;

    private bool _isTyping;
    private string _currentSentence;

    /// <summary>다음 문장 요청 (E키 또는 Next 버튼). Presenter가 구독.</summary>
    public event Action OnNextClicked;
    /// <summary>대화 종료 버튼. Presenter가 구독.</summary>
    public event Action OnEndClicked;
    /// <summary>퀘스트 버튼. 연결 시 Presenter가 구독.</summary>
    public event Action OnQuestButtonClicked;

    private void Awake()
    {
        if (_nextButton != null)
            _nextButton.onClick.AddListener(() => OnNextClicked?.Invoke());
        if (_endButton != null)
            _endButton.onClick.AddListener(() => OnEndClicked?.Invoke());
        if (_questButton != null)
            _questButton.onClick.AddListener(() => OnQuestButtonClicked?.Invoke());
    }

    private static string Sanitize(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        return s.Replace('\u00A0', ' ');
    }

    /// <summary>한 문장 표시. 패널 열고 타이핑. 이미 열려 있으면 내용만 갱신.</summary>
    public void Display(string speakerName, string sentence)
    {
        _panel.SetActive(true);
        _nameText.text = Sanitize(speakerName ?? "");
        _currentSentence = Sanitize(sentence ?? "");
        StopAllCoroutines();
        _isTyping = true;
        StartCoroutine(TypeSentence(_currentSentence));
    }

    /// <summary>타이핑 중이면 즉시 완료하고 true. 아니면 false.</summary>
    public bool TrySkipTyping()
    {
        if (!_isTyping) return false;
        StopAllCoroutines();
        _dialogueText.text = _currentSentence ?? "";
        _isTyping = false;
        return true;
    }

    public void SetQuestButtonVisible(bool visible, string buttonText = "퀘스트")
    {
        if (_questButton != null)
            _questButton.gameObject.SetActive(visible);
        if (_questButtonText != null)
            _questButtonText.text = Sanitize(buttonText);
    }

    public void Close()
    {
        StopAllCoroutines();
        _isTyping = false;
        _panel.SetActive(false);
    }

    private IEnumerator TypeSentence(string sentence)
    {
        _isTyping = true;
        _dialogueText.text = "";
        foreach (char letter in (sentence ?? "").ToCharArray())
        {
            _dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        _isTyping = false;
    }
}
