using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 대화창: 문장 표시 + 다음/끝내기만. 퀘스트 등 다른 버튼은 다른 UI에서.
/// </summary>
public class DialogueView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private GameObject _panel;

    [Header("버튼 (UI_Button 프리팹으로 생성)")]
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private Transform _buttonContainer;

    [Header("버튼 (수동 연결, 프리팹 미사용 시)")]
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _endButton;

    private bool _isTyping;
    private string _currentSentence;

    public event Action OnNextClicked;
    public event Action OnEndClicked;

    private void Awake()
    {
        if (_buttonPrefab != null && _buttonContainer != null)
            CreateButtons();
        else
        {
            if (_nextButton != null)
                _nextButton.onClick.AddListener(() => OnNextClicked?.Invoke());
            if (_endButton != null)
                _endButton.onClick.AddListener(() => OnEndClicked?.Invoke());
        }
    }

    private void CreateButtons()
    {
        var nextGo = Instantiate(_buttonPrefab, _buttonContainer);
        var nextUi = nextGo.GetComponent<UI_Button>();
        if (nextUi != null)
            nextUi.Initialize(null, "다음", () => OnNextClicked?.Invoke());

        var endGo = Instantiate(_buttonPrefab, _buttonContainer);
        var endUi = endGo.GetComponent<UI_Button>();
        if (endUi != null)
            endUi.Initialize(null, "끝내기", () => OnEndClicked?.Invoke());
    }

    private static string Sanitize(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        return s.Replace('\u00A0', ' ');
    }

    public void Display(string speakerName, string sentence)
    {
        _panel.SetActive(true);
        _nameText.text = Sanitize(speakerName ?? "");
        _currentSentence = Sanitize(sentence ?? "");
        StopAllCoroutines();
        _isTyping = true;
        StartCoroutine(TypeSentence(_currentSentence));
    }

    public bool TrySkipTyping()
    {
        if (!_isTyping) return false;
        StopAllCoroutines();
        _dialogueText.text = _currentSentence ?? "";
        _isTyping = false;
        return true;
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
