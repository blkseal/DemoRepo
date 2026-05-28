using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcSpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class NpcPrefabEntry
    {
        public NpcInteractable prefab;
        public float spawnWeight = 1f;
    }

    [Header("Spawn Setup")]
    [SerializeField] private NpcPrefabEntry[] npcPrefabs;
    [SerializeField] private NpcSpawnPoint[] spawnPoints;
    [SerializeField] private NpcTargetPoint[] targetPoints;
    [SerializeField] private TableTargetPoint[] tableTargetPoints;
    [SerializeField] private LeaveRestaurantTargetPoint leaveRestaurantTargetPoint;
    [SerializeField] private string npcTag = "NPC";
    [SerializeField] private int maxGroupSize = 4;
    [SerializeField] private Vector2 spawnIntervalRange = new Vector2(3f, 7f);
    [SerializeField] private bool spawnOnStart = true;

    private readonly List<NpcInteractable> activeNpcs = new List<NpcInteractable>();
    private Coroutine spawnRoutine;

    // When true, don't spawn any more table groups for the rest of this session
    private bool groupsDisabledForSession = false;

    private void Start()
    {
        if (spawnOnStart)
        {
            spawnRoutine = StartCoroutine(SpawnLoop());
        }
    }

    public void RegisterNpc(NpcInteractable npc)
    {
        if (npc == null || activeNpcs.Contains(npc))
        {
            return;
        }

        activeNpcs.Add(npc);
    }

    public void UnregisterNpc(NpcInteractable npc)
    {
        if (npc == null)
        {
            return;
        }

        activeNpcs.Remove(npc);
    }

    private System.Collections.IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (CanSpawnAny())
            {
                SpawnNpcBatch();
            }

            var delay = Random.Range(Mathf.Min(spawnIntervalRange.x, spawnIntervalRange.y), Mathf.Max(spawnIntervalRange.x, spawnIntervalRange.y));
            yield return new WaitForSeconds(delay);
        }
    }

    private bool CanSpawnAny()
    {
        // If there are free table seats and we haven't disabled group spawns, allow spawning (groups preferred)
        if (!groupsDisabledForSession && AnyEmptyTable())
            return true;

        // If any take-away queue has space, allow spawning lone NPCs
        if (AnyAvailableQueueSlots())
            return true;

        return false;
    }

    private void SpawnNpcBatch()
    {
        if (npcPrefabs == null || npcPrefabs.Length == 0 || spawnPoints == null || spawnPoints.Length == 0)
        {
            return;
        }

        var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        if (spawnPoint == null)
        {
            return;
        }

        // Try to spawn a table group if tables are available and groups not disabled
        if (!groupsDisabledForSession)
        {
            var groupSize = Random.Range(1, maxGroupSize + 1);
            if (groupSize > 1)
            {
                var tableTarget = GetFreeTableForGroup(groupSize);
                if (tableTarget != null)
                {
                    SpawnTableGroup(spawnPoint, groupSize, tableTarget);

                    // If after spawning we have no empty tables left, disable groups for the rest of the session
                    if (!AnyEmptyTable())
                    {
                        groupsDisabledForSession = true;
                    }

                    return;
                }
            }
        }

        // Otherwise try spawn a lone take-away NPC but only if there is an available queue slot
        var takeAwayTarget = GetRandomTakeAwayTargetPoint();
        if (takeAwayTarget != null)
        {
            SpawnLoneTakeAwayNpc(spawnPoint);
        }
    }

    private void SpawnLoneTakeAwayNpc(NpcSpawnPoint spawnPoint)
    {
        var prefab = GetRandomNpcPrefab();
        if (prefab == null)
        {
            return;
        }

        var targetPoint = GetRandomTakeAwayTargetPoint();
        if (targetPoint == null)
        {
            // No queue space right now
            return;
        }

        var npc = Instantiate(prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
        SetupNpcCommon(npc);

        npc.SetSpawnManager(this);
        npc.SetLeaveTargetPoint(leaveRestaurantTargetPoint);

        npc.SetTargetPoint(targetPoint);
        var agent = npc.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.SetDestination(targetPoint.FrontPosition);
        }

        RegisterNpc(npc);
    }

    private void SpawnTableGroup(NpcSpawnPoint spawnPoint, int groupSize, TableTargetPoint tableTarget)
    {
        var prefab = GetRandomNpcPrefab();
        if (prefab == null)
        {
            return;
        }

        var groupRootObject = new GameObject("NpcGroup_Table");
        groupRootObject.transform.position = spawnPoint.transform.position;
        groupRootObject.transform.rotation = spawnPoint.transform.rotation;

        var group = groupRootObject.AddComponent<NpcGroupInteractable>();
        group.SetTableZone(tableTarget.TableZone);
        var groupMembers = new List<NpcInteractable>();
        var reservedSeats = tableTarget.ReserveSeats(groupSize);

        if (reservedSeats == null)
        {
            Destroy(groupRootObject);
            return;
        }

        for (var i = 0; i < groupSize; i++)
        {
            var npc = Instantiate(prefab, spawnPoint.transform.position, spawnPoint.transform.rotation, groupRootObject.transform);
            SetupNpcCommon(npc);
            npc.SetGroupRoot(group);
            npc.SetSpawnManager(this);
            npc.SetLeaveTargetPoint(leaveRestaurantTargetPoint);
            npc.SetTableTargetPoint(tableTarget);
            npc.SetReservedSeat(reservedSeats[i]);
            groupMembers.Add(npc);
            RegisterNpc(npc);
        }

        group.SetMembers(groupMembers.ToArray());
    }

    private void SetupNpcCommon(NpcInteractable npc)
    {
        if (npc == null)
        {
            return;
        }

        npc.tag = npcTag;

        var rigidbody = npc.GetComponent<Rigidbody>();
        if (rigidbody == null)
        {
            rigidbody = npc.gameObject.AddComponent<Rigidbody>();
        }

        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        var agent = npc.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.avoidancePriority = Random.Range(30, 70);
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        }
    }

    private NpcInteractable GetRandomNpcPrefab()
    {
        if (npcPrefabs == null || npcPrefabs.Length == 0)
        {
            return null;
        }

        var totalWeight = 0f;
        foreach (var entry in npcPrefabs)
        {
            if (entry != null)
            {
                totalWeight += entry.spawnWeight;
            }
        }

        if (totalWeight <= 0f)
        {
            return null;
        }

        var randomValue = Random.value * totalWeight;
        var currentWeight = 0f;

        foreach (var entry in npcPrefabs)
        {
            if (entry != null && entry.prefab != null)
            {
                currentWeight += entry.spawnWeight;
                if (randomValue <= currentWeight)
                {
                    return entry.prefab;
                }
            }
        }

        return npcPrefabs[npcPrefabs.Length - 1]?.prefab;
    }

    private TableTargetPoint GetFreeTableForGroup(int groupSize)
    {
        if (tableTargetPoints == null)
        {
            return null;
        }

        foreach (var tableTarget in tableTargetPoints)
        {
            if (tableTarget != null && tableTarget.HasFreeSeats(groupSize) && tableTarget.IsEmpty())
            {
                return tableTarget;
            }
        }

        return null;
    }

    private bool AnyEmptyTable()
    {
        if (tableTargetPoints == null)
            return false;

        foreach (var tableTarget in tableTargetPoints)
        {
            if (tableTarget != null && tableTarget.IsEmpty())
                return true;
        }

        return false;
    }

    private NpcTargetPoint GetRandomTakeAwayTargetPoint()
    {
        if (targetPoints == null || targetPoints.Length == 0)
        {
            return null;
        }

        // Find queues with available slots
        var availableQueues = new List<NpcTargetPoint>();
        foreach (var targetPoint in targetPoints)
        {
            if (targetPoint != null && targetPoint.GetQueueCount() < targetPoint.GetMaxQueueSize())
            {
                availableQueues.Add(targetPoint);
            }
        }

        if (availableQueues.Count == 0)
        {
            return null; // No queues with available space
        }

        return availableQueues[Random.Range(0, availableQueues.Count)];
    }

    private bool AnyAvailableQueueSlots()
    {
        if (targetPoints == null || targetPoints.Length == 0)
            return false;

        foreach (var targetPoint in targetPoints)
        {
            if (targetPoint != null && targetPoint.GetQueueCount() < targetPoint.GetMaxQueueSize())
                return true;
        }

        return false;
    }

    // Reset session state (called on GameOver/Reset)
    public void ResetSession()
    {
        groupsDisabledForSession = false;
    }
}