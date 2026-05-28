using UnityEngine;
using UnityEngine.AI;

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
    private bool isLeavingRestaurant;
    private NpcGroupInteractable groupRoot;
    private Collider npcCollider;
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
        if (isSitting && !isLeavingRestaurant)
        {
            return;
        }

        // Update no-collision timer
        if (noCollisionTimer > 0f)
        {
            noCollisionTimer -= Time.deltaTime;
            if (noCollisionTimer <= 0f)
            {
                // Re-enable collisions
                if (npcCollider != null)
                {
                    npcCollider.enabled = true;
                }
                if (agent != null)
                {
                    agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
                }
            }
        }

        // Handle seating logic - FIXED WITH NAVMESH GUARD
        if (tableTargetPoint != null && reservedSitPoint != null && agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f)
            {
                SeatAt(reservedSitPoint.transform, tableTargetPoint, reservedSitPoint);
                return;
            }
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

        // Prevent group NPCs from entering other triggers
        if (groupRoot != null)
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

        // Disable collisions immediately
        if (npcCollider != null)
        {
            npcCollider.enabled = false;
        }

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
    public void ForceLeaveRestaurant()
    {
        // 1. Liberta o estado lógico e impede o Update de interferir
        isLeavingRestaurant = true;
        isSitting = false;

        // 2. Remove o parentesco caso ele esteja acoplado à cadeira
        transform.SetParent(null);

        // 3. Atualiza o Animator para ele levantar e andar
        if (animator != null)
        {
            animator.SetBool("IsSitting", false);
            animator.Play("Walk", 0, 0f); // Força o estado de caminhada
        }

        // 4. Liberta logo a cadeira para o próximo cliente
        if (occupiedSitPoint != null && tableTargetPoint != null)
        {
            tableTargetPoint.ReleaseSeat(occupiedSitPoint);
            occupiedSitPoint = null;
        }

        // 5. Configuração segura do NavMeshAgent
        if (agent != null)
        {
            // ATIVAÇÃO CRUCIAL: Acorda o componente antes de mexer nele
            agent.enabled = true;

            // WARP DE SEGURANÇA: Coloca o NPC ligeiramente atrás/fora da mesa
            Vector3 escapePosition = transform.position - (transform.forward * 0.8f);

            // Valida se a posição de escape toca na NavMesh antes do Warp
            if (NavMesh.SamplePosition(escapePosition, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
            else
            {
                agent.Warp(escapePosition);
            }

            // 6. Só dá ordens se ele estiver devidamente colado à NavMesh
            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;

                if (leaveRestaurantTargetPoint != null)
                {
                    agent.SetDestination(leaveRestaurantTargetPoint.transform.position);
                }
                else if (postAnswerTargetPoint != null)
                {
                    agent.SetDestination(postAnswerTargetPoint.position);
                }
            }
            else
            {
                Debug.LogWarning($"{name} não conseguiu ancorar na NavMesh ao tentar sair!");
            }
        }

        // Sai da fila (caso estivesse nela)
        if (targetPoint != null)
        {
            targetPoint.ExitQueue(this);
        }

        // 7. Destrói o NPC de forma segura após 15 segundos (tempo de caminhar até à porta)
        Destroy(gameObject, 15f);
    }
}