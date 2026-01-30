public interface IInteractable
{
    string GetInteractText(); // 화면에 띄울 메시지 (예: "말 걸기", "아이템 줍기")
    void Interact(Player player);           // 실제 상호작용 로직
}