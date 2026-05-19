using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NpcInteractionUI : MonoBehaviour
{
    private static NpcInteractionUI instance;

    private Canvas canvas;
    private GameObject root;
    private Text questionLabel;
    private Button optionOneButton;
    private Button optionTwoButton;
    private Text optionOneLabel;
    private Text optionTwoLabel;
    private NpcInteractable currentNpc;

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
        if (!IsOpen || currentNpc == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            SelectAnswer(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            SelectAnswer(1);
        }
    }

    public void Show(NpcInteractable npc)
    {
        if (npc == null)
        {
            return;
        }

        currentNpc = npc;
        questionLabel.text = npc.QuestionText;
        optionOneLabel.text = $"1. {npc.OptionOneText}";
        optionTwoLabel.text = $"2. {npc.OptionTwoText}";
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

        currentNpc = null;
    }

    public void SelectAnswer(int index)
    {
        if (currentNpc == null)
        {
            return;
        }

        var npc = currentNpc;
        Hide();
        npc.ResolveAnswer(index);
        PlayerInteractionState.SetLocked(false);
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

        var rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0.5f, 0.5f);
        rootRect.anchorMax = new Vector2(0.5f, 0.5f);
        rootRect.sizeDelta = new Vector2(520f, 220f);
        root.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.8f);

        questionLabel = CreateText("Question", root.transform, new Vector2(0f, 60f), new Vector2(480f, 50f), 24);
        questionLabel.alignment = TextAnchor.MiddleCenter;

        optionOneButton = CreateButton("OptionOne", root.transform, new Vector2(0f, 0f));
        optionTwoButton = CreateButton("OptionTwo", root.transform, new Vector2(0f, -60f));

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
        rect.sizeDelta = new Vector2(420f, 44f);
        rect.anchoredPosition = anchoredPosition;

        var image = buttonObject.GetComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.95f);

        var label = CreateText("Label", buttonObject.transform, Vector2.zero, rect.sizeDelta, 20);
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.black;

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
        text.color = Color.white;
        text.text = string.Empty;
        return text;
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