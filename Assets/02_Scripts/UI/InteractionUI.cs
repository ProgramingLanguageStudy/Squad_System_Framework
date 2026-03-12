using UnityEngine;
using TMPro;

/// <summary>
/// 상호작용 안내 문구 표시 전용 UI. 
/// GameEvents.OnInteractTargetChanged를 구독하여 대상이 바뀔 때마다 자동으로 갱신됩니다.
/// </summary>
public class InteractionUI : MonoBehaviour
{
    [SerializeField] private GameObject _uiPanel;
    [SerializeField] private UITweenFacade _uiFacade;
    [SerializeField] private TextMeshProUGUI _msgText;

    public void Awake()
    {
        var panel = _uiFacade != null ? _uiFacade.gameObject : _uiPanel;
        if (panel != null)
            panel.SetActive(false);
    }

    private void OnEnable()
    {
        // 이벤트 구독: 타겟이 바뀌면 HandleTargetChanged 호출
        GameEvents.OnInteractTargetChanged += HandleTargetChanged;
    }

    private void OnDisable()
    {
        // 이벤트 해제: 메모리 누수 방지
        GameEvents.OnInteractTargetChanged -= HandleTargetChanged;
    }

    private void HandleTargetChanged(IInteractable target)
    {
        // 타겟이 있으면 텍스트를 가져오고, 없으면 빈 문자열 전달
        Refresh(target != null ? target.GetInteractText() : "");
    }

    public void Refresh(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            if (_uiFacade != null)
                _uiFacade.PlayExit();
            else if (_uiPanel != null)
                _uiPanel.SetActive(false);
        }
        else
        {
            _msgText.text = message;
            if (_uiFacade != null)
                _uiFacade.PlayEnter();
            else if (_uiPanel != null)
                _uiPanel.SetActive(true);
        }
    }
}