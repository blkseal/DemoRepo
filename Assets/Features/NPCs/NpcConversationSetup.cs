using UnityEngine;

public static class NpcConversationSetup
{
    public static bool IsTakeAwayAvailable(NpcInteractable npc)
    {
        return npc != null && npc.CustomerActionType == CustomerActionType.TakeAway && npc.TakeAwayTargetPoint != null;
    }
}