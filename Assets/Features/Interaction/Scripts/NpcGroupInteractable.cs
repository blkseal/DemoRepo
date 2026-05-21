using UnityEngine;

public class NpcGroupInteractable : MonoBehaviour, IConversationTarget
{
    [SerializeField] private bool hasNpcType;
    [SerializeField] private NpcType npcType;
    [SerializeField] private string questionText = "The group is ready to order.";
    [SerializeField] private string optionOneText = "Take order";
    [SerializeField] private string optionTwoText = "Come back later";

    private NpcInteractable[] members;

    public bool CanInteract => true;
    public bool ApplyStatusEffects => false;
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
        var conversation = TableConversationDatabase.GetRandomTableConversation(CustomerNpcType);
        if (conversation != null)
        {
            questionText = conversation.Steps != null && conversation.Steps.Length > 0 && conversation.Steps[0] != null
                ? conversation.Steps[0].Question
                : questionText;
        }

        Debug.Log($"Interacted with group {name} ({(members != null ? members.Length : 0)} members)");
        PlayerInteractionState.SetLocked(true);
        NpcInteractionUI.Instance.Show(this);
    }

    public InteractionOutcome ResolveAnswer(int answerIndex)
    {
        Debug.Log($"Group {name} selected answer {answerIndex + 1}");
        return new InteractionOutcome
        {
            ConversationFinished = true,
            InteractionResult = InteractionResult.Neutral,
        };
    }
}