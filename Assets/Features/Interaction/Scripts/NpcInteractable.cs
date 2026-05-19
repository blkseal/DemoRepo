using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NpcInteractable : MonoBehaviour
{
    [SerializeField] private string questionText = "Can I help you?";
    [SerializeField] private string optionOneText = "Answer 1";
    [SerializeField] private string optionTwoText = "Answer 2";
    [SerializeField] private Vector3 bubbleOffset = new Vector3(0f, 2.2f, 0f);
    [SerializeField] private Transform postAnswerTargetPoint;
    [SerializeField] private NavMeshAgent agent;

    private GameObject bubbleRoot;
    private Text bubbleLabel;
    private bool questionVisible;
    private bool answered;

    public string QuestionText => questionText;
    public string OptionOneText => optionOneText;
    public string OptionTwoText => optionTwoText;

    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        CreateBubble();
        HideBubble();
    }

    private void LateUpdate()
    {
        if (!questionVisible || bubbleRoot == null)
        {
            return;
        }

        bubbleRoot.transform.position = transform.position + bubbleOffset;

        var camera = Camera.main;
        if (camera != null)
        {
            bubbleRoot.transform.rotation = Quaternion.LookRotation(bubbleRoot.transform.position - camera.transform.position);
        }
    }

    public void Interact()
    {
        if (answered)
        {
            return;
        }

        Debug.Log($"Interacted with {name}");
        ShowQuestion();
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
        HideBubble();

        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

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

    private void ShowQuestion()
    {
        if (bubbleLabel == null)
        {
            CreateBubble();
        }

        bubbleLabel.text = questionText;
        bubbleRoot.SetActive(true);
        questionVisible = true;
    }

    private void HideBubble()
    {
        questionVisible = false;
        if (bubbleRoot != null)
        {
            bubbleRoot.SetActive(false);
        }
    }

    private void CreateBubble()
    {
        bubbleRoot = new GameObject("QuestionBubble");
        bubbleRoot.transform.SetParent(transform, false);
        bubbleRoot.transform.localPosition = bubbleOffset;

        var canvas = bubbleRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        bubbleRoot.AddComponent<CanvasScaler>();
        bubbleRoot.AddComponent<GraphicRaycaster>();

        var rect = bubbleRoot.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(260f, 90f);
        rect.localScale = Vector3.one * 0.01f;

        var background = new GameObject("Background", typeof(RectTransform), typeof(Image));
        background.transform.SetParent(bubbleRoot.transform, false);
        var backgroundRect = background.GetComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;
        background.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);

        var textObject = new GameObject("Text", typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(background.transform, false);
        var textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10f, 10f);
        textRect.offsetMax = new Vector2(-10f, -10f);

        bubbleLabel = textObject.GetComponent<Text>();
        bubbleLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        bubbleLabel.fontSize = 20;
        bubbleLabel.alignment = TextAnchor.MiddleCenter;
        bubbleLabel.color = Color.white;
        bubbleLabel.text = questionText;
    }
}