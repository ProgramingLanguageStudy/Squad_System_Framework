using UnityEngine;

/// <summary>
/// 패널을 띄우고 닫을 때 커서 표시/숨김을 자동으로 요청하는 View 베이스.
/// 상속 후 패널 열 때 OpenPanel(), 닫을 때 ClosePanel()만 호출하면 됨. 까먹지 않도록 여기서 한 번만 처리.
/// </summary>
public abstract class PanelViewBase : MonoBehaviour
{
    /// <summary> 패널을 연다. 커서 표시 요청 후 OnPanelOpened() 호출. </summary>
    protected void OpenPanel()
    {
        GameEvents.OnCursorShowRequested?.Invoke();
        OnPanelOpened();
    }

    /// <summary> 패널을 닫는다. OnPanelClosed() 호출 후 커서 숨김 요청. </summary>
    protected void ClosePanel()
    {
        OnPanelClosed();
        GameEvents.OnCursorHideRequested?.Invoke();
    }

    /// <summary> 패널 활성화 등 실제 표시 처리. 서브클래스에서 구현. </summary>
    protected virtual void OnPanelOpened() { }

    /// <summary> 패널 비활성화 등 실제 숨김 처리. 서브클래스에서 구현. </summary>
    protected virtual void OnPanelClosed() { }
}
