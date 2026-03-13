using UnityEngine;
using TMPro;

/// <summary>
/// 씬 전환용 로딩 UI. DontDestroyOnLoad와 함께 사용.
/// </summary>
public class SceneTransitionLoadingView : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _statusText;

    public void Show()
    {
        if (_panel != null)
            _panel.SetActive(true);
        ClearText();
    }

    public void Hide()
    {
        if (_panel != null)
            _panel.SetActive(false);
    }

    public void ClearText()
    {
        if (_statusText != null)
            _statusText.text = string.Empty;
    }

    /// <param name="progress">null이면 % 미표시. 0~1이면 % 포함.</param>
    public void UpdateProgress(float? progress, string status)
    {
        if (_statusText == null) return;
        var statusMsg = status ?? string.Empty;
        if (progress.HasValue)
        {
            var p = Mathf.Clamp01(progress.Value);
            var percent = Mathf.RoundToInt(p * 100f);
            _statusText.text = string.IsNullOrEmpty(statusMsg) ? $"{percent}%" : $"{statusMsg} {percent}%";
        }
        else
        {
            _statusText.text = statusMsg;
        }
    }
}
