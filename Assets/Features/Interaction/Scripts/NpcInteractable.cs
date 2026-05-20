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

    public CustomerActionType CustomerActionType => customerActionType;
    public NpcType? CustomerNpcType => hasNpcType ? npcType : null;
    public TakeAwayTargetPoint TakeAwayTargetPoint => takeAwayTargetPoint;
    public bool CanInteract => customerActionType == CustomerActionType.TakeAway && takeAwayTargetPoint != null && Vector3.Distance(transform.position, takeAwayTargetPoint.transform.position) <= interactionDistance;
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

    public void Interact()
    {
        if (!CanInteract || HasActiveConversation)
        {
            return;
        }

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

        if (agent != null && postAnswerTargetPoint != null)
        {
            agent.isStopped = false;
            agent.SetDestination(postAnswerTargetPoint.position);
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