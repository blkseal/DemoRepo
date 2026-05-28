using UnityEngine;
using UnityEngine.UI;

public class InteractionPromptUI : MonoBehaviour
{
    private static InteractionPromptUI instance;

    private Canvas canvas;
    private GameObject root;
    private Text promptLabel;

    public static InteractionPromptUI Instance
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

    private static void CreateInstance()
    {
        var go = new GameObject("InteractionPromptUI");
        DontDestroyOnLoad(go);
        instance = go.AddComponent<InteractionPromptUI>();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        BuildUI();
        Hide();
    }

    public void Show(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            Hide();
            return;
        }

        promptLabel.text = message;
        root.SetActive(true);
    }

    public void Hide()
    {
        if (root != null)
        {
            root.SetActive(false);
        }
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

        root = new GameObject("PromptPanel", typeof(RectTransform), typeof(Image));
        root.transform.SetParent(transform, false);

        var rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0.5f, 0f);
        rootRect.anchorMax = new Vector2(0.5f, 0f);
        rootRect.pivot = new Vector2(0.5f, 0f);
        rootRect.anchoredPosition = new Vector2(0f, 70f);
        rootRect.sizeDelta = new Vector2(340f, 46f);

        var background = root.GetComponent<Image>();
        background.color = new Color(0f, 0f, 0f, 0.8f);

        promptLabel = CreateText("Prompt", root.transform, Vector2.zero, rootRect.sizeDelta, 20);
        promptLabel.alignment = TextAnchor.MiddleCenter;
        promptLabel.horizontalOverflow = HorizontalWrapMode.Wrap;
        promptLabel.verticalOverflow = VerticalWrapMode.Overflow;
        promptLabel.resizeTextForBestFit = true;
        promptLabel.resizeTextMinSize = 14;
        promptLabel.resizeTextMaxSize = 20;
        promptLabel.color = Color.white;
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
}