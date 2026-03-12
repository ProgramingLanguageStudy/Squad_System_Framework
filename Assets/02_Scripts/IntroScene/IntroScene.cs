using System;
using System.Collections;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScene : MonoBehaviour
{
    [SerializeField] IntroSceneView _introSceneView;
    [SerializeField] [Tooltip("ResourceManager Preload에 쓸 라벨")]
    private string _prefabLabel = "Prefab";

    private bool _firebaseReady;

    private void Start()
    {
        _introSceneView.Initialize();
        _introSceneView.OnGameStartRequested += HandleGameStartRequested;
        _introSceneView.OnLogoutRequested += HandleLogoutRequested;
        _introSceneView.OnLoginRequested += HandleLoginRequested;
        _introSceneView.OnSignUpRequested += HandleSignUpRequested;

        StartCoroutine(InitFirebaseAndAuth());
    }

    private void OnDestroy()
    {
        _introSceneView.OnGameStartRequested -= HandleGameStartRequested;
        _introSceneView.OnLogoutRequested -= HandleLogoutRequested;
        _introSceneView.OnLoginRequested -= HandleLoginRequested;
        _introSceneView.OnSignUpRequested -= HandleSignUpRequested;
    }

    private void HandleGameStartRequested()
    {
        StartCoroutine(LoadAndTransition());
    }

    private void HandleLogoutRequested()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        _introSceneView.HideMainPanel();
        _introSceneView.ShowLoginPanel();
    }

    private IEnumerator InitFirebaseAndAuth()
    {
        _introSceneView.UpdateProgress(0f, "준비중...");

        var task = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Result != DependencyStatus.Available)
        {
            Debug.LogError($"Firebase dependency error: {task.Result}");
            _introSceneView.HideLoading();
            _introSceneView.ShowLoginPanel();
            _introSceneView.ShowErrorPanel("Firebase 초기화 실패. 재시도해 주세요.");
            yield break;
        }

        _firebaseReady = true;
        var user = FirebaseAuth.DefaultInstance.CurrentUser;

        _introSceneView.HideLoading();
        if (user != null)
            _introSceneView.ShowMainPanel();
        else
            _introSceneView.ShowLoginPanel();
    }

    private void HandleLoginRequested(string email, string password)
    {
        var validationError = GetValidationError(email, password);
        if (validationError != null)
        {
            _introSceneView.ShowErrorPanel(validationError);
            return;
        }
        if (!_firebaseReady)
        {
            _introSceneView.ShowErrorPanel("Firebase 초기화 실패. 재시도해 주세요.");
            return;
        }
        _introSceneView.SetLoginInteractable(false);
        _introSceneView.HideErrorPanel();

        FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task => OnAuthCompleted(task, "로그인"));
    }

    private void HandleSignUpRequested(string email, string password)
    {
        var validationError = GetValidationError(email, password);
        if (validationError != null)
        {
            _introSceneView.ShowErrorPanel(validationError);
            return;
        }
        if (!_firebaseReady)
        {
            _introSceneView.ShowErrorPanel("Firebase 초기화 실패. 재시도해 주세요.");
            return;
        }
        _introSceneView.SetLoginInteractable(false);
        _introSceneView.HideErrorPanel();

        FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task => OnAuthCompleted(task, "회원가입"));
    }

    /// <summary>입력 검증. 오류 시 메시지 반환, 유효 시 null.</summary>
    private static string GetValidationError(string email, string password)
    {
        if (string.IsNullOrEmpty(email)) return "이메일을 입력하세요.";
        if (string.IsNullOrEmpty(password)) return "비밀번호를 입력하세요.";
        if (password.Length < 6) return "비밀번호는 6자 이상이어야 합니다.";
        return null;
    }

    private void OnAuthCompleted(Task<AuthResult> task, string action)
    {
        _introSceneView.SetLoginInteractable(true);

        if (task.IsCanceled)
        {
            _introSceneView.ShowErrorPanel($"{action}이 취소되었습니다.");
            return;
        }
        if (task.IsFaulted)
        {
            var msg = GetAuthErrorMessage(task.Exception);
            _introSceneView.ShowErrorPanel(msg);
            return;
        }

        _introSceneView.HideLoginPanel();
        _introSceneView.ShowMainPanel();
    }

    private static string GetAuthErrorMessage(AggregateException ex)
    {
        if (ex?.InnerExceptions == null || ex.InnerExceptions.Count == 0)
            return "오류가 발생했습니다. 다시 시도해 주세요.";

        var inner = ex.InnerExceptions[0];
        var authError = (inner as FirebaseException)?.ErrorCode ?? (inner.InnerException as FirebaseException)?.ErrorCode;

        if (authError.HasValue)
        {
            switch ((AuthError)authError.Value)
            {
                case AuthError.InvalidEmail:
                    return "올바른 이메일 형식이 아닙니다.";
                case AuthError.WrongPassword:
                case AuthError.InvalidCredential:
                case AuthError.UserNotFound:
                    return "이메일 또는 비밀번호가 올바르지 않습니다.";
                case AuthError.EmailAlreadyInUse:
                    return "이미 사용 중인 이메일입니다.";
                case AuthError.WeakPassword:
                    return "비밀번호는 6자 이상이어야 합니다.";
                case AuthError.TooManyRequests:
                    return "요청이 너무 많습니다. 잠시 후 다시 시도해 주세요.";
            }
        }

        var msg = (inner.InnerException?.Message ?? inner.Message ?? string.Empty).ToLowerInvariant();
        if (msg.Contains("invalid") && msg.Contains("email")) return "올바른 이메일 형식이 아닙니다.";
        if (msg.Contains("badly formatted") || msg.Contains("bad format")) return "올바른 이메일 형식이 아닙니다.";
        if (msg.Contains("wrong_password") || msg.Contains("invalid_credential") || msg.Contains("user_not_found"))
            return "이메일 또는 비밀번호가 올바르지 않습니다.";
        if (msg.Contains("email_exists") || msg.Contains("email_already_in_use")) return "이미 사용 중인 이메일입니다.";
        if (msg.Contains("weak_password")) return "비밀번호는 6자 이상이어야 합니다.";

        return "오류가 발생했습니다. 다시 시도해 주세요.";
    }

    private IEnumerator LoadAndTransition()
    {
        _introSceneView.ShowLoading();
        _introSceneView.UpdateProgress(0f, "준비중...");

        var gm = GameManager.Instance;
        var dm = gm?.DataManager;
        var rm = FindFirstObjectByType<ResourceManager>();

        if (dm != null)
        {
            yield return dm.InitializeAsync((progress, status) =>
            {
                _introSceneView.UpdateProgress(progress * 0.5f, status);
            });
        }
        else
        {
            yield return null;
        }

        _introSceneView.UpdateProgress(0.5f, "ResourceManager 로드중...");

        if (rm != null)
        {
            yield return rm.PreloadByLabelAsync(_prefabLabel, (progress, status) =>
            {
                _introSceneView.UpdateProgress(0.5f + progress * 0.5f, status);
            });
        }
        else
        {
            _introSceneView.UpdateProgress(1f, "ResourceManager 없음");
            yield return null;
        }

        _introSceneView.UpdateProgress(1f, "로드 완료");
        yield return new WaitForSeconds(0.3f);

        SceneManager.LoadScene("Play");
    }
}
