using UnityEngine;
using System;
using DG.Tweening;

public class UIAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float _duration = 0.25f;
    [SerializeField] private float _startScale = 0.92f;

    private RectTransform _mainPanel;
    private CanvasGroup _mainGroup;
    private bool _isInitialized = false;

    /// <summary>
    /// 외부에서 애니메이션에 필요한 컴포넌트들을 주입합니다.
    /// </summary>
    /// <param name="mainPanel">스케일 애니메이션을 적용할 RectTransform</param>
    /// <param name="mainGroup">투명도 애니메이션을 적용할 CanvasGroup</param>
    public void Initialize(RectTransform mainPanel, CanvasGroup mainGroup)
    {
        _mainPanel = mainPanel;
        _mainGroup = mainGroup;

        _isInitialized = true;
        ResetUI();
    }

    public void ResetUI()
    {
        if (!_isInitialized) return;

        if (_mainGroup != null) _mainGroup.alpha = 0;
        if (_mainPanel != null) _mainPanel.localScale = Vector3.one * _startScale;
    }

    public void PlayOpen(Action onComplete = null)
    {
        if (!_isInitialized) return;

        // 기존 트윈 안전하게 제거
        _mainGroup?.DOKill();
        _mainPanel?.DOKill();

        // 투명도와 스케일 동시 연출
        _mainGroup?.DOFade(1f, _duration).SetEase(Ease.OutCubic);
        _mainPanel?.DOScale(1f, _duration)
            .SetEase(Ease.OutQuint)
            .OnComplete(() => onComplete?.Invoke());
    }

    public void PlayClose(Action onComplete = null)
    {
        if (!_isInitialized) return;

        _mainGroup?.DOKill();
        _mainPanel?.DOKill();

        float closeDur = _duration * 0.8f;

        // 닫힐 때는 투명도와 스케일을 원래대로
        _mainGroup?.DOFade(0f, closeDur).SetEase(Ease.InCubic);
        _mainPanel?.DOScale(_startScale, closeDur)
            .SetEase(Ease.InQuart)
            .OnComplete(() => onComplete?.Invoke());
    }
}