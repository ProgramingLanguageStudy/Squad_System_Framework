using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject _uiPanel;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _msgText;

    private string[] _sentences;
    private int _index;

    public bool IsActive => _uiPanel.activeSelf;

    public void Open(string npcName, string[] sentences)
    {
        _uiPanel.SetActive(true);
        _nameText.text = npcName;
        _sentences = sentences;
        _index = 0;

        ShowCurrentSentence();
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

    public void Close() => _uiPanel.SetActive(false);
}