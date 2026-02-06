using UnityEngine;

/// <summary>
/// 플레이 씬 진입점. 나중에 조율자 역할로 입력·인벤토리·대화·퀘스트 등을 연결할 예정.
/// 현재는 연결 없음.
/// </summary>
public class PlayScene : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("PlayScene: 로드됨. (시스템 연결은 조율자 구현 시 추가)");
    }
}
