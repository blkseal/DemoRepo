using UnityEngine;
using UnityEngine.AI;

public class NpcInteractable : MonoBehaviour
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

    private ConversationSession conversationSession;
    private NpcSpawnManager spawnManager;
    private LeaveRestaurantTargetPoint leaveRestaurantTargetPoint;
    private NpcTargetPoint targetPoint;
    private Vector3 queueSlotPosition;
    private bool hasInteracted;

    public CustomerActionType CustomerActionType => customerActionType;
    public NpcType? CustomerNpcType => hasNpcType ? npcType : null;
    public TakeAwayTargetPoint TakeAwayTargetPoint => takeAwayTargetPoint;
    public bool CanInteract => !hasInteracted && customerActionType == CustomerActionType.TakeAway && IsInInteractionRange();
    public string QuestionText => conversationSession?.CurrentStep?.Question ?? string.Empty;
    public string OptionOneText => GetOptionLabel(0);
    public string OptionTwoText => GetOptionLabel(1);
    public bool HasActiveConversation => conversationSession != null && !conversationSession.IsFinished;

    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
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
        if (targetPoint == null || agent == null)
        {
            return;
        }

        if (!targetPoint.Contains(this))
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f)
            {
                targetPoint.EnterQueue(this);
            }

            return;
        }

        if (targetPoint.IsFront(this))
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;
        if (Vector3.Distance(transform.position, queueSlotPosition) > 0.15f)
        {
            agent.SetDestination(queueSlotPosition);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (leaveRestaurantTargetPoint == null || other == null)
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
        targetPoint = newTargetPoint;
        if (agent != null && targetPoint != null)
        {
            agent.isStopped = false;
            agent.SetDestination(targetPoint.FrontPosition);
        }
    }

    public void SetQueueContext(NpcTargetPoint newTargetPoint, int index, Vector3 slotPosition)
    {
        targetPoint = newTargetPoint;
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
            };
        }

        var finished = conversationSession.ApplyAnswer(answerIndex);
        if (!finished)
        {
            return new InteractionOutcome
            {
                ConversationFinished = false,
                InteractionResult = InteractionResult.Neutral,
            };
        }

        var result = conversationSession.GetInteractionResult();
        Debug.Log($"{name} interaction result: {result}");

        if (targetPoint != null)
        {
            targetPoint.ExitQueue(this);
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
        };
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
}