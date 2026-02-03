using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private GameObject _uiPanel;
    [SerializeField] private TextMeshProUGUI _msgText;

    // ���� Initialize���� Interactor�� ���� �ʿ䰡 �����ϴ�.
    public void Setup()
    {
        _uiPanel.SetActive(false);
    }

    // �ܺ�(PlayScene)���� "�̰� ������"��� �θ� �Լ�
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