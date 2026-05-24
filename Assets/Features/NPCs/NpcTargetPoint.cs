using System.Collections.Generic;
using UnityEngine;

public class NpcTargetPoint : MonoBehaviour
{
    [SerializeField] private List<QueueSlot> queueSlots = new List<QueueSlot>();
    [SerializeField] private Collider triggerCollider;

    private readonly List<NpcInteractable> queue = new List<NpcInteractable>();

    public Vector3 FrontPosition => queueSlots.Count > 0 ? queueSlots[0].Position : transform.position;

    private void Awake()
    {
        if (triggerCollider == null)
        {
            triggerCollider = GetComponent<Collider>();
        }

        if (triggerCollider != null && !triggerCollider.isTrigger)
        {
            triggerCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var npc = other.GetComponent<NpcInteractable>();
        if (npc != null && !Contains(npc) && !IsPartOfGroup(npc))
        {
            EnterQueue(npc);
        }
    }

    private bool IsPartOfGroup(NpcInteractable npc)
    {
        return npc.GetComponent<NpcGroupInteractable>() != null || 
               npc.transform.parent?.GetComponent<NpcGroupInteractable>() != null;
    }

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
        if (index >= 0 && index < queueSlots.Count)
        {
            return queueSlots[index].Position;
        }

        return transform.position;
    }

    public int GetQueueCount()
    {
        return queue.Count;
    }

    public int GetMaxQueueSize()
    {
        return queueSlots.Count;
    }

    private void RefreshQueue()
    {
        queue.RemoveAll(npc => npc == null);

        // Assign each NPC in queue to their corresponding slot position
        for (var i = 0; i < queue.Count; i++)
        {
            var npc = queue[i];
            var slotPosition = GetQueueSlotPosition(i);

            npc.SetQueueContext(this, i, slotPosition);
        }
    }

    public void OnValidate()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        RefreshQueue();
    }
}

[System.Serializable]
public class QueueSlot
{
    [SerializeField] private Transform slotTransform;

    public Vector3 Position => slotTransform != null ? slotTransform.position : Vector3.zero;
    public Transform SlotTransform => slotTransform;
}