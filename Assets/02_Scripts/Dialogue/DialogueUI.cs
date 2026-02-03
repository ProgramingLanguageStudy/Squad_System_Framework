using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private GameObject _panel;

    [Header("버튼")]
    [SerializeField] private Button _endButton;
    [SerializeField] private Button _questButton;
    [SerializeField] private TextMeshProUGUI _questButtonText;

    private bool _isTyping;
    private string[] _sentences;
    private int _currentIndex;

    private void Awake()
    {
        if (_endButton != null)
            _endButton.onClick.AddListener(() => DialogueSystem.Instance?.EndDialogue());
        if (_questButton != null)
            _questButton.onClick.AddListener(() => DialogueSystem.Instance?.RequestQuestDialogue());
    }

    /// <summary>폰트에 없는 문자(\u00A0 등)를 일반 공백으로 치환해 TMP 경고를 막습니다.</summary>
    private static string SanitizeForTMP(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        return s.Replace('\u00A0', ' ');
    }

    public void Open(string name, string[] sentences, bool showQuestButton = false, string questButtonText = "퀘스트")
    {
        _panel.SetActive(true);
        _nameText.text = SanitizeForTMP(name ?? "");
        _sentences = sentences != null ? Array.ConvertAll(sentences, s => SanitizeForTMP(s ?? "")) : Array.Empty<string>();
        _currentIndex = 0;

        if (_questButton != null)
            _questButton.gameObject.SetActive(showQuestButton);
        if (_questButtonText != null)
            _questButtonText.text = SanitizeForTMP(questButtonText ?? "퀘스트");

        UpdateUI();
    }

    /// <summary>퀘스트 버튼 클릭 후 대사만 바꿀 때 사용.</summary>
    public void ReplaceContent(string speakerName, string[] sentences)
    {
        _nameText.text = SanitizeForTMP(speakerName ?? "");
        _sentences = sentences != null ? Array.ConvertAll(sentences, s => SanitizeForTMP(s ?? "")) : Array.Empty<string>();
        _currentIndex = 0;
        StopAllCoroutines();
        _isTyping = false;
        UpdateUI();
    }

    public void SetQuestButtonVisible(bool visible)
    {
        if (_questButton != null)
            _questButton.gameObject.SetActive(visible);
    }

    public bool ShowNext()
    {
        if (_isTyping)
        {
            StopAllCoroutines();
            _dialogueText.text = SanitizeForTMP(_sentences[_currentIndex]);
            _isTyping = false;
            return false;
        }

        _currentIndex++;
        if (_currentIndex >= _sentences.Length) return true;

        StartCoroutine(TypeSentence(SanitizeForTMP(_sentences[_currentIndex])));
        return false;
    }

    private IEnumerator TypeSentence(string sentence)
    {
        _isTyping = true;
        _dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            _dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        _isTyping = false;
    }

    private void UpdateUI()
    {
        if (_sentences != null && _sentences.Length > 0 && _currentIndex < _sentences.Length)
            _dialogueText.text = SanitizeForTMP(_sentences[_currentIndex]);
    }

    public void Close() => _panel.SetActive(false);
}
