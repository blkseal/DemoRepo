using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class NpcInteractable : MonoBehaviour, IConversationTarget
{
    [Header("Interaction Setup")]
    [SerializeField] private CustomerActionType customerActionType = CustomerActionType.TakeAway;
    [SerializeField] private bool hasNpcType;
    [SerializeField] private NpcType npcType;
    [SerializeField] private TakeAwayTargetPoint takeAwayTargetPoint;
    [SerializeField] private float interactionDistance = 2f;

    [Header("Movement")]
    [SerializeField] private Transform postAnswerTargetPoint;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Header("Seating")]
    [SerializeField] private float sittingYOffset = 0f;
    [SerializeField] private float sittingForwardOffset = 0f;

    private ConversationSession conversationSession;
    private NpcSpawnManager spawnManager;
    private LeaveRestaurantTargetPoint leaveRestaurantTargetPoint;
    private NpcTargetPoint targetPoint;
    private TableTargetPoint tableTargetPoint;
    private TableSitPoint occupiedSitPoint;
    private TableSitPoint reservedSitPoint;
    private Vector3 queueSlotPosition;
    private bool hasInteracted;
    private bool isSitting;
    private bool isLeaving;      // true when standing up and walking to exit
    public bool IsLeaving => isLeaving;
    private NpcGroupInteractable groupRoot;
    private Collider npcCollider;

    // Track ignored NPC colliders so we can restore collisions later
    private readonly List<Collider> ignoredNpcColliders = new List<Collider>();

    private float noCollisionTimer = 0f;
    private const float NO_COLLISION_DURATION = 10f;

    public CustomerActionType CustomerActionType => customerActionType;
    public NpcType? CustomerNpcType => hasNpcType ? npcType : null;
    public TakeAwayTargetPoint TakeAwayTargetPoint => takeAwayTargetPoint;
    public bool CanInteract => groupRoot == null && !hasInteracted && customerActionType == CustomerActionType.TakeAway && IsInInteractionRange();
    public bool ApplyStatusEffects => true;
    public bool UseReviewResolver => true;
    public string ConversationTitleText => string.Empty;
    public string QuestionText => conversationSession?.CurrentStep?.Question ?? string.Empty;
    public string OptionOneText => GetOptionLabel(0);
    public string OptionTwoText => GetOptionLabel(1);
    public bool HasActiveConversation => conversationSession != null && !conversationSession.IsFinished;
    public bool IsSitting => isSitting;

    public void SetGroupRoot(NpcGroupInteractable newGroupRoot)
    {
        groupRoot = newGroupRoot;
    }

    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (npcCollider == null)
        {
            npcCollider = GetComponent<Collider>();
        }
    }

    private void Start()
    {
        if (agent != null)
        {
            agent.avoidancePriority = Random.Range(30, 70);
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        }
    }

    private void Update()
    {
        if (isSitting)
        {
            return;
        }

        // Update no-collision timer
        if (noCollisionTimer > 0f)
        {
            noCollisionTimer -= Time.deltaTime;
            if (noCollisionTimer <= 0f)
            {
                // Restore ignored collisions
                RestoreIgnoredNpcCollisions();
                if (agent != null)
                {
                    agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
                }
            }
        }

        // Handle seating logic - UNCHANGED
        if (tableTargetPoint != null && reservedSitPoint != null && agent != null && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f)
        {
            SeatAt(reservedSitPoint.transform, tableTargetPoint, reservedSitPoint);
            return;
        }

        if (tableTargetPoint != null && reservedSitPoint == null && agent != null && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f)
        {
            if (tableTargetPoint.TrySeatNpc(this))
            {
                return;
            }
        }

        if (targetPoint == null || agent == null)
        {
            return;
        }

        // Queue management - simplified with automatic slot assignment from NpcTargetPoint
        if (!targetPoint.Contains(this))
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f)
            {
                targetPoint.EnterQueue(this);
            }

            return;
        }

        // Always move to the assigned queue slot position
        agent.isStopped = false;
        if (Vector3.Distance(transform.position, queueSlotPosition) > 0.15f)
        {
            agent.SetDestination(queueSlotPosition);
        }
        else
        {
            agent.isStopped = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (leaveRestaurantTargetPoint == null || other == null)
        {
            return;
        }

        // Block group NPCs UNLESS they are actively walking to the exit
        if (groupRoot != null && !isLeaving)
        {
            return;
        }

        if (other.GetComponentInParent<LeaveRestaurantTargetPoint>() == leaveRestaurantTargetPoint)
        {
            Despawn();
        }
    }

    public void SetSpawnManager(NpcSpawnManager manager)
    {
        spawnManager = manager;
    }

    public void SetLeaveTargetPoint(LeaveRestaurantTargetPoint targetPoint)
    {
        leaveRestaurantTargetPoint = targetPoint;
    }

    public void SetTargetPoint(NpcTargetPoint newTargetPoint)
    {
        tableTargetPoint = null;
        reservedSitPoint = null;
        targetPoint = newTargetPoint;
        if (agent != null && targetPoint != null)
        {
            agent.isStopped = false;
            agent.SetDestination(targetPoint.FrontPosition);
        }
    }

    public void SetTableTargetPoint(TableTargetPoint newTableTargetPoint)
    {
        targetPoint = null;
        tableTargetPoint = newTableTargetPoint;
        if (agent != null && tableTargetPoint != null)
        {
            agent.isStopped = false;
            agent.SetDestination(tableTargetPoint.transform.position);
        }
    }

    public void SetReservedSeat(TableSitPoint sitPoint)
    {
        reservedSitPoint = sitPoint;
    }

    public void SetQueueContext(NpcTargetPoint queueTargetPoint, int index, Vector3 slotPosition)
    {
        targetPoint = queueTargetPoint;
        tableTargetPoint = null;
        reservedSitPoint = null;
        queueSlotPosition = slotPosition;

        if (!HasActiveConversation && agent != null)
        {
            if (targetPoint != null && targetPoint.IsFront(this))
            {
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(queueSlotPosition);
            }
        }
    }

    public void Interact()
    {
        if (!CanInteract || HasActiveConversation)
        {
            return;
        }

        hasInteracted = true;

        var conversation = TakeAwayConversationDatabase.GetRandomTakeAwayConversation(CustomerNpcType);
        if (conversation == null)
        {
            Debug.LogWarning($"{name} has no Take Away conversation defined yet.");
            return;
        }

        conversationSession = new ConversationSession(conversation);
        Debug.Log($"Interacted with {name}");
        PlayerInteractionState.SetLocked(true);
        NpcInteractionUI.Instance.Show(this);
    }

    public InteractionOutcome ResolveAnswer(int answerIndex)
    {
        if (conversationSession == null)
        {
            return new InteractionOutcome
            {
                ConversationFinished = true,
                InteractionResult = InteractionResult.Neutral,
                SuspicionDelta = 0,
                ReviewPointsDelta = 0,
            };
        }

        var finished = conversationSession.ApplyAnswer(answerIndex);
        if (!finished)
        {
            return new InteractionOutcome
            {
                ConversationFinished = false,
                InteractionResult = InteractionResult.Neutral,
                SuspicionDelta = 0,
                ReviewPointsDelta = 0,
            };
        }

        var result = conversationSession.GetInteractionResult();
        var interactionStrength = conversationSession.AccumulatedAnswerValue;
        Debug.Log($"{name} interaction result: {result}");

        if (targetPoint != null)
        {
            targetPoint.ExitQueue(this);
            targetPoint = null;
        }

        // Start no-collision timer
        noCollisionTimer = NO_COLLISION_DURATION;

        // Instead of disabling our collider entirely (which would prevent triggers such as LeaveRestaurantTargetPoint),
        // ignore collisions only with other NPC colliders so we can walk through them while still triggering leave areas.
        StartIgnoreNpcCollisions();

        if (agent != null)
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        }

        if (agent != null && postAnswerTargetPoint != null)
        {
            agent.isStopped = false;
            agent.SetDestination(postAnswerTargetPoint.position);
        }
        else if (agent != null && leaveRestaurantTargetPoint != null)
        {
            agent.isStopped = false;
            agent.SetDestination(leaveRestaurantTargetPoint.transform.position);
        }
        else
        {
            Debug.LogWarning($"{name} has no post-answer target point assigned.");
        }

        conversationSession = null;

        return new InteractionOutcome
        {
            ConversationFinished = true,
            InteractionResult = result,
            SuspicionDelta = interactionStrength,
            ReviewPointsDelta = 0,
        };
    }

    // Called by external systems (e.g. WaitTimeSystem) to force this NPC to leave immediately.
    // Handles both seated and standing NPCs.
    public void ForceLeaveRestaurant()
    {
        if (isLeaving) return;
        isLeaving = true;

        // Cancel any active conversation and mark as interacted so player can't reopen dialog
        conversationSession = null;
        hasInteracted = true;

        // If seated, release the seat and update animator
        if (isSitting)
        {
            if (occupiedSitPoint != null && tableTargetPoint != null)
            {
                tableTargetPoint.ReleaseSeat(occupiedSitPoint);
                occupiedSitPoint = null;
            }

            isSitting = false;
            if (animator != null)
                animator.SetBool("IsSitting", false);
        }

        // Exit any queue context
        if (targetPoint != null)
        {
            targetPoint.ExitQueue(this);
            targetPoint = null;
        }

        // Ensure NavMeshAgent is enabled to walk away
        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
        }

        // When forced to leave, make sure we can pass through other NPCs
        noCollisionTimer = NO_COLLISION_DURATION;
        StartIgnoreNpcCollisions();
        if (agent != null)
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        }

        // Send to exit or despawn immediately if no exit
        if (leaveRestaurantTargetPoint != null && agent != null)
        {
            agent.SetDestination(leaveRestaurantTargetPoint.transform.position);
        }
        else
        {
            Despawn();
        }
    }

    private void StartIgnoreNpcCollisions()
    {
        if (npcCollider == null)
            return;

        // Find all NPCs in scene and ignore collisions with their colliders
        var others = Object.FindObjectsOfType<NpcInteractable>();
        foreach (var other in others)
        {
            if (other == null || other == this)
                continue;

            var otherCol = other.npcCollider;
            if (otherCol == null)
                continue;

            // Ignore collisions between this and other NPC
            Physics.IgnoreCollision(npcCollider, otherCol, true);
            if (!ignoredNpcColliders.Contains(otherCol))
                ignoredNpcColliders.Add(otherCol);
        }
    }

    private void RestoreIgnoredNpcCollisions()
    {
        if (npcCollider == null)
            return;

        foreach (var otherCol in ignoredNpcColliders)
        {
            if (otherCol == null)
                continue;

            Physics.IgnoreCollision(npcCollider, otherCol, false);
        }

        ignoredNpcColliders.Clear();
    }

    private void MoveToExit()
    {
        if (agent == null)
        {
            return;
        }

        if (postAnswerTargetPoint != null)
        {
            agent.isStopped = false;
            agent.SetDestination(postAnswerTargetPoint.position);
        }
        else if (leaveRestaurantTargetPoint != null)
        {
            agent.isStopped = false;
            agent.SetDestination(leaveRestaurantTargetPoint.transform.position);
        }
    }

    public void SeatAt(Transform seatTransform, TableTargetPoint sourceTableTargetPoint, TableSitPoint sitPoint)
    {
        if (seatTransform == null)
        {
            return;
        }

        isSitting = true;
        tableTargetPoint = sourceTableTargetPoint;
        occupiedSitPoint = sitPoint;

        var seatPosition = seatTransform.position + Vector3.up * sittingYOffset + seatTransform.forward * sittingForwardOffset;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.Warp(seatPosition);
            agent.enabled = false;
        }

        transform.position = seatPosition;
        transform.rotation = seatTransform.rotation;

        if (animator != null)
        {
            animator.SetBool("IsSitting", true);
        }
    }

    public void Despawn()
    {
        if (spawnManager != null)
        {
            spawnManager.UnregisterNpc(this);
        }

        if (targetPoint != null)
        {
            targetPoint.ExitQueue(this);
        }

        if (occupiedSitPoint != null && tableTargetPoint != null)
        {
            tableTargetPoint.ReleaseSeat(occupiedSitPoint);
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Stands the NPC up from the chair and walks them to the restaurant exit.
    /// The NPC is destroyed automatically when it reaches LeaveRestaurantTargetPoint.
    /// </summary>
    public void StandUpAndLeave()
    {
        if (isLeaving) return;
        isLeaving = true;

        // Release the seat so other groups can sit here
        if (occupiedSitPoint != null && tableTargetPoint != null)
        {
            tableTargetPoint.ReleaseSeat(occupiedSitPoint);
            occupiedSitPoint = null;
        }

        // Stand up
        isSitting = false;
        if (animator != null)
        {
            animator.SetBool("IsSitting", false);
        }

        // Re-enable NavMeshAgent
        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
        }

        // Make sure leaving NPC can pass through other NPCs
        noCollisionTimer = NO_COLLISION_DURATION;
        StartIgnoreNpcCollisions();
        if (agent != null)
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        }

        // Walk to the exit
        if (leaveRestaurantTargetPoint != null && agent != null)
        {
            agent.SetDestination(leaveRestaurantTargetPoint.transform.position);
        }
        else
        {
            // No exit point configured – just despawn immediately
            Despawn();
        }
    }

    private bool IsInInteractionRange()
    {
        var referencePoint = takeAwayTargetPoint != null ? takeAwayTargetPoint.transform.position : transform.position;
        return Vector3.Distance(transform.position, referencePoint) <= interactionDistance;
    }

    private string GetOptionLabel(int index)
    {
        var step = conversationSession?.CurrentStep;
        if (step == null || step.Options == null || index < 0 || index >= step.Options.Length || step.Options[index] == null)
        {
            return string.Empty;
        }

        return step.Options[index].Label;
    }
}