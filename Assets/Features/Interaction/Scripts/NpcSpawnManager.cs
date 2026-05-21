using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcSpawnManager : MonoBehaviour
{
    [Header("Spawn Setup")]
    [SerializeField] private NpcInteractable npcPrefab;
    [SerializeField] private NpcSpawnPoint[] spawnPoints;
    [SerializeField] private NpcTargetPoint[] targetPoints;
    [SerializeField] private TableTargetPoint[] tableTargetPoints;
    [SerializeField] private LeaveRestaurantTargetPoint leaveRestaurantTargetPoint;
    [SerializeField] private string npcTag = "NPC";
    [SerializeField] private int maxNpcCount = 10;
    [SerializeField] private int maxGroupSize = 4;
    [SerializeField] private Vector2 spawnIntervalRange = new Vector2(3f, 7f);
    [SerializeField] private bool spawnOnStart = true;

    private readonly List<NpcInteractable> activeNpcs = new List<NpcInteractable>();
    private Coroutine spawnRoutine;

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

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (activeNpcs.Count < maxNpcCount)
            {
                SpawnNpcBatch();
            }

            var delay = Random.Range(Mathf.Min(spawnIntervalRange.x, spawnIntervalRange.y), Mathf.Max(spawnIntervalRange.x, spawnIntervalRange.y));
            yield return new WaitForSeconds(delay);
        }
    }

    private void SpawnNpcBatch()
    {
        if (npcPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            return;
        }

        var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        if (spawnPoint == null)
        {
            return;
        }

        var groupSize = Random.Range(1, maxGroupSize + 1);
        if (groupSize > 1)
        {
            var tableTarget = GetFreeTableForGroup(groupSize);
            if (tableTarget != null)
            {
                SpawnTableGroup(spawnPoint, groupSize, tableTarget);
                return;
            }
        }

        SpawnLoneTakeAwayNpc(spawnPoint);
    }

    private void SpawnLoneTakeAwayNpc(NpcSpawnPoint spawnPoint)
    {
        var npc = Instantiate(npcPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
        SetupNpcCommon(npc);

        npc.SetSpawnManager(this);
        npc.SetLeaveTargetPoint(leaveRestaurantTargetPoint);
        npc.SetTargetPoint(GetRandomTakeAwayTargetPoint());
        RegisterNpc(npc);
    }

    private void SpawnTableGroup(NpcSpawnPoint spawnPoint, int groupSize, TableTargetPoint tableTarget)
    {
        var groupRootObject = new GameObject("NpcGroup_Table");
        groupRootObject.transform.position = spawnPoint.transform.position;
        groupRootObject.transform.rotation = spawnPoint.transform.rotation;

        var group = groupRootObject.AddComponent<NpcGroupInteractable>();
        var groupMembers = new List<NpcInteractable>();
        var reservedSeats = tableTarget.ReserveSeats(groupSize);

        if (reservedSeats == null)
        {
            Destroy(groupRootObject);
            return;
        }

        for (var i = 0; i < groupSize; i++)
        {
            var npc = Instantiate(npcPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation, groupRootObject.transform);
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

    private NpcTargetPoint GetRandomTakeAwayTargetPoint()
    {
        if (targetPoints == null || targetPoints.Length == 0)
        {
            return null;
        }

        return targetPoints[Random.Range(0, targetPoints.Length)];
    }
}