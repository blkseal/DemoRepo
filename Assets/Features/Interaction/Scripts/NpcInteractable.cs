using UnityEngine;
using UnityEngine.AI;

public class NpcInteractable : MonoBehaviour
{
    [SerializeField] private string questionText = "Can I help you?";
    [SerializeField] private string optionOneText = "Answer 1";
    [SerializeField] private string optionTwoText = "Answer 2";
    [SerializeField] private Transform postAnswerTargetPoint;
    [SerializeField] private NavMeshAgent agent;

    public string QuestionText => questionText;
    public string OptionOneText => optionOneText;
    public string OptionTwoText => optionTwoText;

    private bool answered;

    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    public void Interact()
    {
        if (answered)
        {
            return;
        }

        Debug.Log($"Interacted with {name}");
        PlayerInteractionState.SetLocked(true);
        NpcInteractionUI.Instance.Show(this);
    }

    public void ResolveAnswer(int answerIndex)
    {
        if (answered)
        {
            return;
        }

        answered = true;
        Debug.Log($"{name} received answer {answerIndex + 1}");

        if (agent != null && postAnswerTargetPoint != null)
        {
            agent.isStopped = false;
            agent.SetDestination(postAnswerTargetPoint.position);
        }
        else
        {
            Debug.LogWarning($"{name} has no post-answer target point assigned.");
        }
    }
}