using UnityEngine;

/// <summary>
/// OnNpcInteracted 구독 → Loader로 해당 npcId 대화 조회 → OnPlayDialogueRequested 발행.
/// DialogueSystem은 그대로 "받은 대화만 재생". NPC는 npcId만 알림.
/// </summary>
public class NpcDialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueDataLoader _loader;

    private void OnEnable()
    {
        GameEvents.OnNpcInteracted += HandleNpcInteracted;
    }

    private void OnDisable()
    {
        GameEvents.OnNpcInteracted -= HandleNpcInteracted;
    }

    private void HandleNpcInteracted(string npcId)
    {
        if (_loader == null || !_loader.IsLoaded) return;

        var data = _loader.GetBestForNpc(npcId);
        if (data != null)
            GameEvents.OnPlayDialogueRequested?.Invoke(data);
    }
}
