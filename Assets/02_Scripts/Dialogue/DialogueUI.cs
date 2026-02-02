using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject _uiPanel;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _msgText;

    private string[] _sentences;
    private int _index;

    public bool IsActive => _uiPanel.activeSelf;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsActive)
        {
            Next();
        }
    }

    public void Open(string npcName, string[] sentences)
    {
        _uiPanel.SetActive(true);
        _nameText.text = npcName;
        _sentences = sentences;
        _index = 0;

        ShowCurrentSentence();

        // [중요] 대화 시작 시점에 시스템에 "지금 대화 중"이라고 알려줘야 함
        DialogueSystem.Instance.IsTalking = true;
    }

    public void Next()
    {
        _index++;
        if (_index < _sentences.Length)
        {
            ShowCurrentSentence();
        }
        else
        {
            Close();
        }
    }

    private void ShowCurrentSentence()
    {
        _msgText.text = _sentences[_index];
    }

    public void Close()
    {
        _uiPanel.SetActive(false);

        // [핵심] 대화가 완전히 끝났으니 시스템에 보고함 (여기서 Step이 올라감)
        DialogueSystem.Instance.IsTalking = false; // 대화 종료 알림
        DialogueSystem.Instance.OnDialogueComplete();
    }
}