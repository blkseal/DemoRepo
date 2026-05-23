using System.Collections.Generic;
using UnityEngine;

public class NpcGroupInteractable : MonoBehaviour, IConversationTarget
{
    [SerializeField] private bool hasNpcType;
    [SerializeField] private NpcType npcType;

    private NpcInteractable[] members;
    private TableConversationDatabase.TableConversationData conversationData;
    private bool awaitingOrder;
    private bool orderCompleted;
    private string questionText = "We're ready to order.";
    private string optionOneText = "What will it be?";
    private string optionTwoText = "Give me a minute.";

    public bool CanInteract => true;
    public bool ApplyStatusEffects => true;
    public bool UseReviewResolver => false;
    public NpcType? CustomerNpcType => hasNpcType ? npcType : null;
    public string QuestionText => questionText;
    public string OptionOneText => optionOneText;
    public string OptionTwoText => optionTwoText;

    public void SetMembers(NpcInteractable[] groupMembers)
    {
        members = groupMembers;
    }

    public void Interact()
    {
        if (orderCompleted)
        {
            questionText = "We've already ordered";
            optionOneText = "Ok";
            optionTwoText = string.Empty;
        }
        else
        {
            awaitingOrder = false;
            conversationData = TableConversationDatabase.CreateConversation(members != null && members.Length > 0 ? members.Length : 1);
            questionText = conversationData.IntroQuestion;
            optionOneText = conversationData.IntroPositiveOption;
            optionTwoText = conversationData.IntroNegativeOption;
        }

        Debug.Log($"Interacted with group {name} ({(members != null ? members.Length : 0)} members)");
        PlayerInteractionState.SetLocked(true);
        NpcInteractionUI.Instance.Show(this);
    }

    public InteractionOutcome ResolveAnswer(int answerIndex)
    {
        if (orderCompleted)
        {
            return new InteractionOutcome
            {
                ConversationFinished = true,
                InteractionResult = InteractionResult.Neutral,
                SuspicionDelta = 0,
                ReviewPointsDelta = 0,
            };
        }

        if (conversationData == null)
        {
            return new InteractionOutcome
            {
                ConversationFinished = true,
                InteractionResult = InteractionResult.Neutral,
                SuspicionDelta = 0,
                ReviewPointsDelta = 0,
            };
        }

        if (!awaitingOrder)
        {
            if (answerIndex == 0)
            {
                awaitingOrder = true;
                questionText = conversationData.OrderQuestion;
                optionOneText = conversationData.PositiveOrderOption;
                optionTwoText = conversationData.NegativeOrderOption;

                return new InteractionOutcome
                {
                    ConversationFinished = false,
                    InteractionResult = InteractionResult.Neutral,
                    SuspicionDelta = 0,
                    ReviewPointsDelta = 0,
                };
            }

            return new InteractionOutcome
            {
                ConversationFinished = true,
                InteractionResult = InteractionResult.Negative,
                SuspicionDelta = 2,
                ReviewPointsDelta = 0,
            };
        }

        orderCompleted = true;

        var isPositiveAnswer = answerIndex == 0;
        return new InteractionOutcome
        {
            ConversationFinished = true,
            InteractionResult = isPositiveAnswer ? InteractionResult.Positive : InteractionResult.Negative,
            SuspicionDelta = isPositiveAnswer ? 0 : 2,
            ReviewPointsDelta = 0,
        };
    }
}