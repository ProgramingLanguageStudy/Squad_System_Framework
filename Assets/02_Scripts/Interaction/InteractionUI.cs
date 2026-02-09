using UnityEngine;
using TMPro;

/// <summary>상호작용 안내 문구만 표시. GameEvents.OnInteractTargetChanged 구독해 자동 갱신.</summary>
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
    private void OnEnable()
    {
        GameEvents.OnInteractTargetChanged += HandleTargetChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnInteractTargetChanged -= HandleTargetChanged;
    }

    private void HandleTargetChanged(IInteractable target)
    {
        Refresh(target != null ? target.GetInteractText() : "");
    }

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