using System.Collections;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI _nameText;
    [SerializeField] private TMPro.TextMeshProUGUI _dialogueText;
    [SerializeField] private GameObject _panel;

    private bool _isTyping; // 현재 글자가 타이핑 중인가?
    private string[] _sentences;
    private int _currentIndex;

    public void Open(string name, string[] sentences)
    {
        _panel.SetActive(true);
        _nameText.text = name;
        _sentences = sentences;
        _currentIndex = 0;

        UpdateUI();
    }

    public bool ShowNext()
    {
        // 1. 만약 글자가 아직 타이핑 중이라면?
        if (_isTyping)
        {
            StopAllCoroutines(); // 타이핑 멈추고
            _dialogueText.text = _sentences[_currentIndex]; // 전체 문장 즉시 표시
            _isTyping = false;
            return false; // 아직 '끝'은 아니니까 false 반환
        }

        // 2. 글자가 이미 다 보여진 상태라면? 다음 문장으로!
        _currentIndex++;
        if (_currentIndex >= _sentences.Length) return true; // 진짜 끝!

        StartCoroutine(TypeSentence(_sentences[_currentIndex]));
        return false;
    }

    private IEnumerator TypeSentence(string sentence)
    {
        _isTyping = true;
        _dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            _dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f); // 찰나의 대기 (타이핑 속도)
        }
        _isTyping = false;
    }

    private void UpdateUI()
    {
        _dialogueText.text = _sentences[_currentIndex];
    }

    public void Close() => _panel.SetActive(false);
}