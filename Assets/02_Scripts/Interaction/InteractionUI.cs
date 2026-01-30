using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private GameObject _uiPanel;
    [SerializeField] private TextMeshProUGUI _msgText;

    // 이제 Initialize에서 Interactor를 받을 필요가 없습니다.
    public void Setup()
    {
        _uiPanel.SetActive(false);
    }

    // 외부(PlayScene)에서 "이걸 보여줘"라고 부를 함수
    public void Refresh(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            _uiPanel.SetActive(false);
        }
        else
        {
            _msgText.text = message;
            _uiPanel.SetActive(true);
        }
    }
}