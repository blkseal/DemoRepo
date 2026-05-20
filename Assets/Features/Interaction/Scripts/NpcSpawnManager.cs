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
    [SerializeField] private LeaveRestaurantTargetPoint leaveRestaurantTargetPoint;
    [SerializeField] private string npcTag = "NPC";
    [SerializeField] private int maxNpcCount = 10;
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
                SpawnNpc();
            }

            var delay = Random.Range(Mathf.Min(spawnIntervalRange.x, spawnIntervalRange.y), Mathf.Max(spawnIntervalRange.x, spawnIntervalRange.y));
            yield return new WaitForSeconds(delay);
        }
    }

    private void SpawnNpc()
    {
        if (npcPrefab == null || spawnPoints == null || spawnPoints.Length == 0 || targetPoints == null || targetPoints.Length == 0)
        {
            return;
        }

        var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        if (spawnPoint == null)
        {
            return;
        }

        var targetPoint = targetPoints[Random.Range(0, targetPoints.Length)];
        if (targetPoint == null)
        {
            return;
        }

        var npc = Instantiate(npcPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
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
            agent.SetDestination(targetPoint.FrontPosition);
        }

        npc.SetSpawnManager(this);
        npc.SetLeaveTargetPoint(leaveRestaurantTargetPoint);
        npc.SetTargetPoint(targetPoint);
        RegisterNpc(npc);
    }
}