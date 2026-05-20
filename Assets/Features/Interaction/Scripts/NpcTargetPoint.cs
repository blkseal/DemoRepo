using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcTargetPoint : MonoBehaviour
{
    [SerializeField] private Transform queueDirection;
    [SerializeField] private float queueSpacing = 1.25f;
    [SerializeField] private float navMeshSampleRadius = 1f;

    private readonly List<NpcInteractable> queue = new List<NpcInteractable>();

    public Vector3 FrontPosition => transform.position;

    public bool Contains(NpcInteractable npc)
    {
        return queue.Contains(npc);
    }

    public void EnterQueue(NpcInteractable npc)
    {
        if (npc == null || queue.Contains(npc))
        {
            return;
        }

        queue.Add(npc);
        RefreshQueue();
    }

    public void ExitQueue(NpcInteractable npc)
    {
        if (npc == null)
        {
            return;
        }

        if (queue.Remove(npc))
        {
            RefreshQueue();
        }
    }

    public bool IsFront(NpcInteractable npc)
    {
        return queue.Count > 0 && queue[0] == npc;
    }

    public Vector3 GetQueueSlotPosition(int index)
    {
        var direction = queueDirection != null ? queueDirection.forward : transform.forward;
        var desiredPosition = transform.position - direction.normalized * queueSpacing * index;

        if (NavMesh.SamplePosition(desiredPosition, out var hit, navMeshSampleRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return desiredPosition;
    }

    private void RefreshQueue()
    {
        queue.RemoveAll(npc => npc == null);

        for (var i = 0; i < queue.Count; i++)
        {
            queue[i].SetQueueContext(this, i, GetQueueSlotPosition(i));
        }
    }
}