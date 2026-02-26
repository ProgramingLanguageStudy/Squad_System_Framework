using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 인터페이스 사용을 위해 추가
using DG.Tweening;

public class MapView : PanelViewBase, IDragHandler, IPointerDownHandler
{
    [SerializeField] private GameObject _mapPanel;
    [SerializeField] private Camera _mapCamera;
    [SerializeField] private RectTransform _mapRectTransform;
    [SerializeField] private CanvasGroup _mapCanvasGroup;
    [SerializeField] UIAnimation _uiAnim;
    [SerializeField] private Slider _zoomSlider;

    [Header("Map Settings")]
    [SerializeField] private float _mapSize = 300f;    // 전체 맵 크기 (100x3)
    [SerializeField] private float _dragSensitivity = 0.5f; // 드래그 감도
    private Vector2 _mapCenter; // 초기화 시 자동으로 받아올 중앙 좌표

    [Header("Zoom Settings")]
    [SerializeField] private float _minSize = 50f;
    [SerializeField] private float _maxSize = 150f;
    [SerializeField] private float _zoomSpeed = 0.1f;

    public void Initialize()
    {
        if (_mapPanel != null)
            _mapPanel.SetActive(false);

        if (_mapCamera != null)
        {
            _mapCamera.enabled = false;
            _mapCamera.orthographicSize = _maxSize;

            // [중앙 좌표 자동 설정] 
            // 초기 카메라 위치를 맵의 중앙으로 간주합니다.
            _mapCenter = new Vector2(_mapCamera.transform.position.x, _mapCamera.transform.position.z);
        }

        _uiAnim.Initialize(_mapRectTransform, _mapCanvasGroup);

        if (_zoomSlider != null)
        {
            _zoomSlider.minValue = _minSize;
            _zoomSlider.maxValue = _maxSize;
            _zoomSlider.value = _mapCamera.orthographicSize;
            _zoomSlider.onValueChanged.RemoveAllListeners();
            _zoomSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    // IDragHandler 인터페이스 구현
    public void OnDrag(PointerEventData eventData)
    {
        if (_mapCamera == null || !_mapPanel.activeSelf) return;

        // 줌 배율에 따라 드래그 속도를 보정 (확대 시 더 세밀하게 이동)
        float zoomFactor = _mapCamera.orthographicSize / _maxSize;

        // 마우스 이동 방향과 지형 이동 방향을 일치시킴 (Inverse Drag)
        Vector3 move = new Vector3(-eventData.delta.x, 0, -eventData.delta.y) * _dragSensitivity * zoomFactor;

        _mapCamera.transform.position += move;

        // 드래그 즉시 경계선 체크
        ApplyBoundaryLimit();
    }

    // 이벤트를 받기 위해 필요한 인터페이스
    public void OnPointerDown(PointerEventData eventData) { }

    public void ToggleMap()
    {
        if (_mapPanel == null) return;
        bool isOpening = !_mapPanel.activeSelf;

        if (isOpening)
        {
            _mapPanel.SetActive(true);
            if (_mapCamera != null) _mapCamera.enabled = true;
            _uiAnim?.PlayOpen();
            OpenPanel();
        }
        else
        {
            _uiAnim?.PlayClose(() =>
            {
                _mapPanel.SetActive(false);
                if (_mapCamera != null) _mapCamera.enabled = false;
            });
            ClosePanel();
        }
    }

    public void ScrollMap(Vector2 input)
    {
        if (_mapCamera == null || !_mapPanel.activeSelf) return;

        float scrollY = input.y;
        if (Mathf.Abs(scrollY) < 0.01f) return;

        float targetSize = _mapCamera.orthographicSize - (scrollY * _zoomSpeed);
        targetSize = Mathf.Clamp(targetSize, _minSize, _maxSize);

        ApplyZoom(targetSize);
    }

    private void ApplyZoom(float targetSize)
    {
        _mapCamera.DOKill();
        // 줌이 변하는 도중에도 실시간으로 경계선을 체크하여 파란 배경 노출 방지
        _mapCamera.DOOrthoSize(targetSize, 0.2f)
            .SetEase(Ease.OutCubic)
            .OnUpdate(ApplyBoundaryLimit);

        if (_zoomSlider != null)
            _zoomSlider.SetValueWithoutNotify(targetSize);
    }

    private void OnSliderValueChanged(float value)
    {
        if (_mapCamera != null)
        {
            _mapCamera.orthographicSize = value;
            ApplyBoundaryLimit();
        }
    }

    /// <summary>
    /// 카메라가 맵 경계 밖으로 나가지 않도록 좌표를 고정합니다.
    /// </summary>
    private void ApplyBoundaryLimit()
    {
        if (_mapCamera == null) return;

        float currentSize = _mapCamera.orthographicSize;
        float halfMap = _mapSize * 0.5f;
        float aspect = _mapCamera.aspect;

        // 현재 줌 사이즈에서 이동 가능한 한계 거리 계산
        float xLimit = Mathf.Max(0, halfMap - (currentSize * aspect));
        float zLimit = Mathf.Max(0, halfMap - currentSize);

        // 자동 갱신된 _mapCenter를 기준으로 Clamp
        float clampedX = Mathf.Clamp(_mapCamera.transform.position.x, _mapCenter.x - xLimit, _mapCenter.x + xLimit);
        float clampedZ = Mathf.Clamp(_mapCamera.transform.position.z, _mapCenter.y - zLimit, _mapCenter.y + zLimit);

        _mapCamera.transform.position = new Vector3(clampedX, _mapCamera.transform.position.y, clampedZ);
    }
}