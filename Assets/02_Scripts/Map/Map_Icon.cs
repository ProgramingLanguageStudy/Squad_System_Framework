using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using DG.Tweening;

// 추상 클래스로 선언하여 자식들이 기능을 채우게 합니다.
public abstract class Map_Icon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Common UI References")]
    [SerializeField] protected RectTransform _iconRoot;
    [SerializeField] protected Image _iconImage;

    [Header("Common Tween Settings")]
    [SerializeField] private float _hoverScale = 1.2f;
    [SerializeField] private float _clickScale = 0.9f;
    [SerializeField] private float _duration = 0.2f;

    // --- 추상 메서드: 자식들이 각자의 기능을 여기에 작성함 ---
    protected abstract void OnIconClicked();

    // --- 공통 연출 로직 (모든 아이콘 공통) ---

    public void OnPointerEnter(PointerEventData eventData)
    {
        _iconRoot.DOKill(); // 기존 트윈 중복 방지
        _iconRoot.DOScale(_hoverScale, _duration).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _iconRoot.DOKill();
        _iconRoot.DOScale(1.0f, _duration).SetEase(Ease.OutQuad);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _iconRoot.DOScale(_clickScale, 0.1f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 클릭 연출 (살짝 튀어오름)
        _iconRoot.DOScale(_hoverScale, 0.1f);
        _iconRoot.DOPunchPosition(Vector3.up * 10f, 0.3f, 10, 1f);

        // [핵심] 자식이 오버라이드한 구체적인 기능을 호출
        OnIconClicked();
    }
}