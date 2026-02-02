using UnityEngine;

public class Mushroom : MonoBehaviour
{
    [SerializeField] private ItemData _data; // 아까 만든 Mushroom SO를 여기에 드래그 앤 드롭!

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 인벤토리에 데이터 전달!
            InventoryManager.Instance.AddItem(_data, 1);

            Destroy(gameObject); // 월드에서 아이템 제거
        }
    }
}