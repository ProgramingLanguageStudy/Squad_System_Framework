using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroSceneView : MonoBehaviour
{
    [Header("메인 (씬 기본. 로그인됐을 때만 표시)")]
    [SerializeField] Button _gameStartButton;
    [SerializeField] Button _logoutButton;

    [Header("타이틀 패널 (playEnterOnStart 체크한 Facade 붙이면 자동 연출)")]
    [SerializeField] UITweenFacade _titleFacade;

    [Header("로그인 패널")]
    [SerializeField] GameObject _loginPanel;
    [SerializeField] UITweenFacade _loginFacade;
    [SerializeField] TMP_InputField _emailInput;
    [SerializeField] TMP_InputField _passwordInput;
    [SerializeField] Button _loginButton;
    [SerializeField] Button _signUpButton;

    [Header("에러 패널 (로그인 패널 내부)")]
    [SerializeField] GameObject _errorPanel;
    [SerializeField] UITweenFacade _errorFacade;
    [SerializeField] TextMeshProUGUI _errorText;
    [SerializeField] Button _errorCloseButton;

    [Header("메인 패널 (Game Start, Logout)")]
    [SerializeField] UITweenFacade _mainFacade;

    [Header("로딩 UI")]
    [SerializeField] GameObject _loadingPanel;
    [SerializeField] UITweenFacade _loadingFacade;
    [SerializeField] Slider _loadingSlider;
    [SerializeField] TextMeshProUGUI _loadingStatusText;

    public event Action OnGameStartRequested;
    public event Action OnLogoutRequested;
    public event Action<string, string> OnLoginRequested;
    public event Action<string, string> OnSignUpRequested;

    public void Initialize()
    {
        _gameStartButton?.onClick.AddListener(() => OnGameStartRequested?.Invoke());
        _logoutButton?.onClick.AddListener(() => OnLogoutRequested?.Invoke());
        _loginButton?.onClick.AddListener(HandleLoginClick);
        _signUpButton?.onClick.AddListener(HandleSignUpClick);
        _errorCloseButton?.onClick.AddListener(HideErrorPanel);

        if (_mainFacade != null)
            _mainFacade.gameObject.SetActive(false);
        else
        {
            if (_gameStartButton != null) _gameStartButton.gameObject.SetActive(false);
            if (_logoutButton != null) _logoutButton.gameObject.SetActive(false);
        }
        if (_loginFacade != null)
            _loginFacade.gameObject.SetActive(false);
        else if (_loginPanel != null)
            _loginPanel.SetActive(false);
        if (_errorFacade != null)
            _errorFacade.gameObject.SetActive(false);
        else if (_errorPanel != null)
            _errorPanel.SetActive(false);
        if (_loadingFacade != null)
            _loadingFacade.gameObject.SetActive(true);
        else if (_loadingPanel != null)
            _loadingPanel.SetActive(true);
    }

    private void HandleLoginClick()
    {
        var (email, password) = GetInput();
        OnLoginRequested?.Invoke(email, password);
    }

    private void HandleSignUpClick()
    {
        var (email, password) = GetInput();
        OnSignUpRequested?.Invoke(email, password);
    }

    private (string email, string password) GetInput()
    {
        var email = _emailInput != null ? _emailInput.text.Trim() : string.Empty;
        var password = _passwordInput != null ? _passwordInput.text : string.Empty;
        return (email, password);
    }

    /// <summary>에러 패널 표시. 닫기 버튼으로 HideErrorPanel 호출.</summary>
    public void ShowErrorPanel(string message)
    {
        if (_errorText != null) _errorText.text = message ?? string.Empty;
        if (_errorFacade != null)
            _errorFacade.PlayEnter();
        else if (_errorPanel != null)
            _errorPanel.SetActive(true);
    }

    /// <summary>에러 패널 숨김.</summary>
    public void HideErrorPanel()
    {
        if (_errorFacade != null)
            _errorFacade.PlayExit();
        else if (_errorPanel != null)
            _errorPanel.SetActive(false);
    }

    /// <summary>로그인 UI 활성화 여부 (로딩 중 비활성화용).</summary>
    public void SetLoginInteractable(bool interactable)
    {
        if (_loginButton != null) _loginButton.interactable = interactable;
        if (_signUpButton != null) _signUpButton.interactable = interactable;
        if (_emailInput != null) _emailInput.interactable = interactable;
        if (_passwordInput != null) _passwordInput.interactable = interactable;
    }

    /// <summary>로그인 패널 표시.</summary>
    public void ShowLoginPanel()
    {
        if (_loginFacade != null)
            _loginFacade.PlayEnter();
        else if (_loginPanel != null)
            _loginPanel.SetActive(true);
        HideErrorPanel();
    }

    /// <summary>로그인 패널 숨김.</summary>
    public void HideLoginPanel()
    {
        if (_loginFacade != null)
            _loginFacade.PlayExit();
        else if (_loginPanel != null)
            _loginPanel.SetActive(false);
        HideErrorPanel();
    }

    /// <summary>메인(Game Start + Logout) 표시. 로그인됐을 때.</summary>
    public void ShowMainPanel()
    {
        if (_mainFacade != null)
            _mainFacade.PlayEnter();
        else
        {
            if (_gameStartButton != null) _gameStartButton.gameObject.SetActive(true);
            if (_logoutButton != null) _logoutButton.gameObject.SetActive(true);
        }
        HideLoginPanel();
    }

    /// <summary>메인(Game Start + Logout) 숨김.</summary>
    public void HideMainPanel()
    {
        if (_mainFacade != null)
            _mainFacade.PlayExit();
        else
        {
            if (_gameStartButton != null) _gameStartButton.gameObject.SetActive(false);
            if (_logoutButton != null) _logoutButton.gameObject.SetActive(false);
        }
    }

    /// <summary>로딩 패널 표시 (Game Start 시 DataManager/Preload 진행용).</summary>
    public void ShowLoading()
    {
        HideMainPanel();
        if (_loadingFacade != null)
            _loadingFacade.PlayEnter();
        else if (_loadingPanel != null)
            _loadingPanel.SetActive(true);
    }

    /// <summary>로딩 패널 숨김 (초기 로그인 확인 완료 시).</summary>
    public void HideLoading()
    {
        if (_loadingFacade != null)
            _loadingFacade.PlayExit();
        else if (_loadingPanel != null)
            _loadingPanel.SetActive(false);
    }

    /// <summary>로딩 진행률·상태 갱신.</summary>
    public void UpdateProgress(float progress, string status)
    {
        if (_loadingSlider != null)
            _loadingSlider.value = Mathf.Clamp01(progress);
        if (_loadingStatusText != null)
            _loadingStatusText.text = status ?? string.Empty;
    }
}
