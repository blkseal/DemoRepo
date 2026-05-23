using UnityEngine;

public class TakeAwayTargetPoint : NpcTargetPoint
{
    [SerializeField] private Transform[] postConversationTargetPoints;

    public Transform[] PostConversationTargetPoints => postConversationTargetPoints;
}