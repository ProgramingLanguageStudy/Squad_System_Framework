using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 재사용 버튼. 아이콘/텍스트 표시 + 클릭 시 넘겨받은 콜백만 실행. GameEvents는 참조하지 않음.
/// </summary>
public class UI_Button : MonoBehaviour
{
    [SerializeField] Button _button;
    [SerializeField] Image _buttonIcon;
    [SerializeField] TextMeshProUGUI _buttonText;

    /// <summary>
    /// 재사용 시 기존 리스너 제거 후 새로 설정. icon/text null이면 빈 상태로 둠.
    /// </summary>
    /// <param name="icon">아이콘 스프라이트. null이면 표시 안 함.</param>
    /// <param name="buttonText">버튼 텍스트. null/빈 문자열 가능.</param>
    /// <param name="onClick">클릭 시 호출할 콜백. null이면 리스너 안 붙임.</param>
    public void Initialize(Sprite icon, string buttonText, Action onClick)
    {
        ResetSlot();

        _buttonIcon.sprite = icon;
        _buttonText.text = buttonText ?? string.Empty;
        if (onClick != null)
            _button.onClick.AddListener(() => onClick.Invoke());
    }

    /// <summary>
    /// 슬롯 비우기. 아이콘/텍스트/리스너 제거. 비활성화는 호출 쪽에서 처리.
    /// </summary>
    public void ResetSlot()
    {
        _buttonIcon.sprite = null;
        _buttonText.text = string.Empty;
        _button.onClick.RemoveAllListeners();
    }
}
