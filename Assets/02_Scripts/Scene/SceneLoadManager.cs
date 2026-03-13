using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬 전환 로딩. GameManager 하위에서 OnSceneReady 후 로딩 UI 숨김.
/// </summary>
public class SceneLoadManager : MonoBehaviour
{
    [SerializeField] private SceneTransitionLoadingView _loadingView;
    [SerializeField] [Tooltip("ResourceManager Preload에 쓸 라벨")]
    private string _prefabLabel = "Prefab";

    private void OnDestroy()
    {
        PlayScene.OnSceneReady -= HandleSceneReady;
    }

    /// <summary>씬 로드 (DataManager, ResourceManager, Play 씬). 로딩 UI는 OnSceneReady 후 숨김.</summary>
    public void LoadPlayScene()
    {
        StartCoroutine(LoadPlaySceneRoutine());
    }

    private IEnumerator LoadPlaySceneRoutine()
    {
        _loadingView?.Show();
        _loadingView?.UpdateProgress(0f, "준비중...");

        var gm = GameManager.Instance;
        var dm = gm?.DataManager;
        var rm = gm?.ResourceManager;

        if (dm != null)
        {
            yield return dm.InitializeAsync((progress, status) =>
            {
                _loadingView?.UpdateProgress(progress * 0.5f, status);
            });
        }
        else
        {
            yield return null;
        }

        _loadingView?.UpdateProgress(0.5f, "ResourceManager 로드중...");

        if (rm != null)
        {
            yield return rm.PreloadByLabelAsync(_prefabLabel, (progress, status) =>
            {
                _loadingView?.UpdateProgress(0.5f + progress * 0.5f, status);
            });
        }
        else
        {
            _loadingView?.UpdateProgress(1f, "ResourceManager 없음");
            yield return null;
        }

        var loadOp = SceneManager.LoadSceneAsync("Play");
        loadOp.allowSceneActivation = false;
        while (loadOp.progress < 0.9f)
        {
            _loadingView?.UpdateProgress(0.5f + loadOp.progress / 0.9f * 0.5f, "씬 준비중...");
            yield return null;
        }
        _loadingView?.UpdateProgress(1f, "로드 완료");
        yield return new WaitForSeconds(0.3f);

        PlayScene.OnSceneReady += HandleSceneReady;
        loadOp.allowSceneActivation = true;
    }

    private void HandleSceneReady()
    {
        PlayScene.OnSceneReady -= HandleSceneReady;
        _loadingView?.Hide();
    }
}
