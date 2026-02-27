//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class PortalIcon : MonoBehaviour
//{
//    [SerializeField] private Image _iconImage;
//    [SerializeField] private TextMeshProUGUI _nameText;

//    private Portal _myPortal;
//    private IInteractReceiver _player;
//    private System.Action<Portal, IInteractReceiver> _onClickCallback;

//    public void Setup(Portal portal, IInteractReceiver player, System.Action<Portal, IInteractReceiver> onClick)
//    {
//        _myPortal = portal;
//        _player = player;
//        _onClickCallback = onClick;

//        // PortalData에 설정한 이름과 아이콘 적용
//        if (_nameText != null) _nameText.text = portal.DisplayName;
//        if (_iconImage != null && portal.Data != null) _iconImage.sprite = portal.Data.icon;

//        // 버튼 클릭 이벤트 연결
//        GetComponent<Button>().onClick.AddListener(() => _onClickCallback?.Invoke(_myPortal, _player));
//    }

//    // 줌 배율에 따라 스케일을 역보정하는 함수 (MapView에서 호출 예정)
//    public void SetScale(float scale)
//    {
//        transform.localScale = new Vector3(scale, scale, 1f);
//    }
//}