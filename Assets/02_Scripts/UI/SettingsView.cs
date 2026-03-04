using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsView : PanelViewBase
{
    [SerializeField] GameObject _settingsPanel;
    [SerializeField] Button _closeButton;
    [SerializeField] Button _saveButton;
    [SerializeField] Button _introButton;
    [SerializeField] Button _quitButton;

    public void Initialize()
    {
        _settingsPanel.SetActive(false);
        _closeButton.onClick.AddListener(() => ToggleSettings());
        _saveButton.onClick.AddListener(() => OnSaveButtonClicked());
        _introButton.onClick.AddListener(() => OnIntroButtonClicked());
        _quitButton.onClick.AddListener(() => OnQuitButtonClicked());
    }

    /// <summary>설정 토글. PlayScene 입력에서 Request.</summary>
    public void RequestToggle() => ToggleSettings();

    private void ToggleSettings()
    {
        if (_settingsPanel == null) return;
        bool isOpening = !_settingsPanel.activeSelf;

        if (isOpening)
        {
            OpenPanel();
        }
        else
        {
            ClosePanel();
        }
    }

    protected override void OnPanelOpened()
    {
        _settingsPanel.SetActive(true);
    }

    protected override void OnPanelClosed()
    {
        _settingsPanel.SetActive(false);
    }

    public void OnSaveButtonClicked()
    {
        GameManager.Instance.SaveManager.Save();
    }

    public void OnIntroButtonClicked()
    {
        GameManager.Instance.SaveManager.Save();
        SceneManager.LoadScene("Intro");
    }

    public void OnQuitButtonClicked()
    {
        GameManager.Instance.SaveManager.Save();
        Application.Quit();
    }
}
