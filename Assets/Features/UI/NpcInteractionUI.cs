using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NpcInteractionUI : MonoBehaviour
{
    private static NpcInteractionUI instance;

    private Canvas canvas;
    private GameObject root;
    private RectTransform rootRect;
    private Text titleLabel;
    private RectTransform titleRect;
    private Text questionLabel;
    private RectTransform questionRect;
    private Button optionOneButton;
    private Button optionTwoButton;
    private Text optionOneLabel;
    private Text optionTwoLabel;
    private IConversationTarget currentTarget;

    public static NpcInteractionUI Instance
    {
        get
        {
            if (instance == null)
            {
                CreateInstance();
            }

            return instance;
        }
    }

    public bool IsOpen => root != null && root.activeSelf;

    private static void CreateInstance()
    {
        var go = new GameObject("NpcInteractionUI");
        DontDestroyOnLoad(go);
        instance = go.AddComponent<NpcInteractionUI>();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        EnsureEventSystem();
        BuildUI();
        Hide();
    }

    private void Update()
    {
        if (!IsOpen || currentTarget == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            SelectAnswer(0);
        }
        else if (optionTwoButton.gameObject.activeSelf && (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)))
        {
            SelectAnswer(1);
        }
    }

    public void Show(IConversationTarget target)
    {
        if (target == null)
        {
            return;
        }

        currentTarget = target;
        titleLabel.text = target.ConversationTitleText;
        questionLabel.text = target.QuestionText;
        optionOneLabel.text = $"1. {target.OptionOneText}";
        optionTwoLabel.text = string.IsNullOrWhiteSpace(target.OptionTwoText) ? string.Empty : $"2. {target.OptionTwoText}";

        titleLabel.gameObject.SetActive(!string.IsNullOrWhiteSpace(target.ConversationTitleText));
        optionTwoButton.gameObject.SetActive(!string.IsNullOrWhiteSpace(target.OptionTwoText));

        FitText(titleRect, titleLabel);
        FitText(questionRect, questionLabel);
        FitButtonText(optionOneLabel);
        FitButtonText(optionTwoLabel);
        LayoutButtons();

        root.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Hide()
    {
        if (root != null)
        {
            root.SetActive(false);
        }

        currentTarget = null;
    }

    public void SelectAnswer(int index)
    {
        if (currentTarget == null)
        {
            return;
        }

        var target = currentTarget;
        var outcome = target.ResolveAnswer(index);

        if (outcome.ConversationFinished)
        {
            var statusSystem = GameStatusSystem.Instance;
            if (statusSystem != null)
            {
                if (target.UseReviewResolver)
                {
                    statusSystem.ApplyInteractionResult(outcome.InteractionResult, outcome.SuspicionDelta, target.CustomerNpcType);
                }
                else
                {
                    statusSystem.ApplyInteractionResult(outcome.SuspicionDelta, outcome.ReviewPointsDelta);
                }
            }

            Hide();
            PlayerInteractionState.SetLocked(false);
            return;
        }

        Show(target);
    }

    private void BuildUI()
    {
        canvas = gameObject.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
        }

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        if (gameObject.GetComponent<CanvasScaler>() == null)
        {
            gameObject.AddComponent<CanvasScaler>();
        }

        if (gameObject.GetComponent<GraphicRaycaster>() == null)
        {
            gameObject.AddComponent<GraphicRaycaster>();
        }

        root = new GameObject("Panel", typeof(RectTransform), typeof(Image));
        root.transform.SetParent(transform, false);

        rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0.5f, 0.5f);
        rootRect.anchorMax = new Vector2(0.5f, 0.5f);
        rootRect.sizeDelta = new Vector2(560f, 320f);
        root.GetComponent<Image>().color = new Color(0.96f, 0.86f, 0.68f, 1f);

        titleLabel = CreateText("Title", root.transform, new Vector2(0f, 125f), new Vector2(520f, 32f), 22);
        titleRect = titleLabel.GetComponent<RectTransform>();
        titleLabel.alignment = TextAnchor.MiddleCenter;
        titleLabel.horizontalOverflow = HorizontalWrapMode.Wrap;
        titleLabel.verticalOverflow = VerticalWrapMode.Overflow;
        titleLabel.resizeTextForBestFit = true;
        titleLabel.resizeTextMinSize = 16;
        titleLabel.resizeTextMaxSize = 22;

        questionLabel = CreateText("Question", root.transform, new Vector2(0f, 80f), new Vector2(520f, 80f), 24);
        questionRect = questionLabel.GetComponent<RectTransform>();
        questionLabel.alignment = TextAnchor.MiddleCenter;
        questionLabel.horizontalOverflow = HorizontalWrapMode.Wrap;
        questionLabel.verticalOverflow = VerticalWrapMode.Overflow;
        questionLabel.resizeTextForBestFit = true;
        questionLabel.resizeTextMinSize = 16;
        questionLabel.resizeTextMaxSize = 24;

        optionOneButton = CreateButton("OptionOne", root.transform, new Vector2(0f, -10f));
        optionTwoButton = CreateButton("OptionTwo", root.transform, new Vector2(0f, -110f));

        optionOneLabel = optionOneButton.GetComponentInChildren<Text>();
        optionTwoLabel = optionTwoButton.GetComponentInChildren<Text>();

        optionOneButton.onClick.AddListener(() => SelectAnswer(0));
        optionTwoButton.onClick.AddListener(() => SelectAnswer(1));
    }

    private Button CreateButton(string name, Transform parent, Vector2 anchoredPosition)
    {
        var buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        var rect = buttonObject.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(520f, 70f); // maior e mais elegante
        rect.anchoredPosition = anchoredPosition;

        var image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.90f, 0.90f, 0.90f, 1f); // cinza suave

        // Bordas do botão
        var outline = buttonObject.AddComponent<Outline>();
        outline.effectColor = new Color(0f, 0f, 0f, 0.45f);
        outline.effectDistance = new Vector2(3f, -3f);

        var label = CreateText("Label", buttonObject.transform, Vector2.zero, rect.sizeDelta, 26);
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.black;
        label.fontStyle = FontStyle.Bold;

        return buttonObject.GetComponent<Button>();
    }


    private static Text CreateText(string name, Transform parent, Vector2 anchoredPosition, Vector2 size, int fontSize)
    {
        var textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(parent, false);

        var rect = textObject.GetComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPosition;

        var text = textObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.color = Color.black;
        text.fontStyle = FontStyle.Bold;
        text.text = string.Empty;
        return text;
    }

    private void FitText(RectTransform rect, Text text)
    {
        if (rect == null || text == null)
        {
            return;
        }

        Canvas.ForceUpdateCanvases();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, Mathf.Max(rect.sizeDelta.y, text.preferredHeight + 10f));
    }

    private void FitButtonText(Text label)
    {
        if (label == null)
        {
            return;
        }

        label.resizeTextForBestFit = true;
        label.resizeTextMinSize = 14;
        label.resizeTextMaxSize = 20;
        label.horizontalOverflow = HorizontalWrapMode.Wrap;
        label.verticalOverflow = VerticalWrapMode.Overflow;
    }

    private void LayoutButtons()
    {
        var questionHeight = questionRect != null ? questionRect.sizeDelta.y : 80f;
        var questionBottom = 80f - questionHeight;

        if (optionTwoButton.gameObject.activeSelf)
        {
            optionOneButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, questionBottom - 35f);
            optionTwoButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, questionBottom - 95f);
        }
        else
        {
            optionOneButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, questionBottom - 65f);
        }
    }

    private static void EnsureEventSystem()
    {
        if (EventSystem.current != null)
        {
            return;
        }

        var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        DontDestroyOnLoad(eventSystem);
    }
}