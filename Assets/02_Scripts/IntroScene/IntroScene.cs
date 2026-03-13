using System;
using System.Collections;
using UnityEngine;

public class IntroScene : MonoBehaviour
{
    [SerializeField] IntroSceneView _introSceneView;

    private IntroAuthService _authService;

    private void Start()
    {
        _authService = new IntroAuthService();

        _introSceneView.Initialize();
        _introSceneView.OnGameStartRequested += HandleGameStartRequested;
        _introSceneView.OnLogoutRequested += HandleLogoutRequested;
        _introSceneView.OnLoginRequested += HandleLoginRequested;
        _introSceneView.OnSignUpRequested += HandleSignUpRequested;

        StartCoroutine(RunIntroSequence());
    }

    private void OnDestroy()
    {
        _introSceneView.OnGameStartRequested -= HandleGameStartRequested;
        _introSceneView.OnLogoutRequested -= HandleLogoutRequested;
        _introSceneView.OnLoginRequested -= HandleLoginRequested;
        _introSceneView.OnSignUpRequested -= HandleSignUpRequested;
    }

    private IEnumerator RunIntroSequence()
    {
        var titleDone = false;
        var authDone = false;
        var authSuccess = false;
        var authHasUser = false;
        var authError = (string)null;

        _introSceneView.ShowTitle(() => titleDone = true);
        StartCoroutine(_authService.InitializeAsync(
            (p, s) => _introSceneView.UpdateProgress(p, s),
            (success, hasUser, errorMessage) =>
            {
                authDone = true;
                authSuccess = success;
                authHasUser = hasUser;
                authError = errorMessage;
            }));

        yield return new WaitUntil(() => titleDone);

        void ApplyAuthResult()
        {
            if (!authSuccess)
            {
                _introSceneView.ShowLoginPanel();
                _introSceneView.ShowErrorPanel(authError);
                return;
            }
            if (authHasUser)
                _introSceneView.ShowMainPanel();
            else
                _introSceneView.ShowLoginPanel();
        }

        if (authDone)
        {
            ApplyAuthResult();
            yield break;
        }

        _introSceneView.ShowLoading(clearText: false);
        yield return new WaitUntil(() => authDone);
        _introSceneView.HideLoading(ApplyAuthResult);
    }

    private void HandleGameStartRequested()
    {
        _introSceneView.PlayTransitionToPlayScene(() =>
        {
            GameManager.Instance?.SceneLoadManager?.LoadPlayScene();
        });
    }

    private void HandleLogoutRequested()
    {
        _authService.SignOut();
        _introSceneView.HideMainPanel();
        _introSceneView.ShowLoginPanel();
    }

    private void HandleLoginRequested(string email, string password)
    {
        _introSceneView.SetLoginInteractable(false);

        _authService.SignIn(email, password,
            onSuccess: () =>
            {
                _introSceneView.SetLoginInteractable(true);
                _introSceneView.HideLoginPanel();
                _introSceneView.ShowMainPanel();
            },
            onError: msg =>
            {
                _introSceneView.SetLoginInteractable(true);
                _introSceneView.ShowErrorPanel(msg);
            });
    }

    private void HandleSignUpRequested(string email, string password)
    {
        _introSceneView.SetLoginInteractable(false);

        _authService.SignUp(email, password,
            onSuccess: () =>
            {
                _introSceneView.SetLoginInteractable(true);
                _introSceneView.HideLoginPanel();
                _introSceneView.ShowMainPanel();
            },
            onError: msg =>
            {
                _introSceneView.SetLoginInteractable(true);
                _introSceneView.ShowErrorPanel(msg);
            });
    }

}
