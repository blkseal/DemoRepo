using UnityEngine;
using UnityEngine.UI;

public class GameStatusHUD : MonoBehaviour
{
    private static GameStatusHUD instance;

    private Canvas canvas;
    private Text suspicionText;
    private Text reviewTitleText;
    private Text reviewDescriptionText;

    public static GameStatusHUD EnsureInstance()
    {
        if (instance != null)
        {
            return instance;
        }

        var existing = UnityEngine.Object.FindFirstObjectByType<GameStatusHUD>();
        if (existing != null)
        {
            instance = existing;
            return instance;
        }

        var go = new GameObject("GameStatusHUD");
        DontDestroyOnLoad(go);
        instance = go.AddComponent<GameStatusHUD>();
        return instance;
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
        UpdateDisplay(0, 0, null);
    }

    private void OnEnable()
    {
        if (GameStatusSystem.Instance != null)
        {
            GameStatusSystem.Instance.StatusChanged += UpdateDisplay;
            var system = GameStatusSystem.Instance;
            UpdateDisplay(system.SuspicionLevel, system.ReviewPoints, null);
        }
    }

    private void OnDisable()
    {
        if (GameStatusSystem.Instance != null)
        {
            GameStatusSystem.Instance.StatusChanged -= UpdateDisplay;
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

        var panel = new GameObject("Panel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(transform, false);

        var panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0f, 1f);
        panelRect.anchorMax = new Vector2(0f, 1f);
        panelRect.pivot = new Vector2(0f, 1f);
        panelRect.anchoredPosition = new Vector2(20f, -20f);
        panelRect.sizeDelta = new Vector2(280f, 130f);
        panel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.55f);

        suspicionText = CreateText("Suspicion", panel.transform, new Vector2(20f, -20f), new Vector2(330f, 30f), 24);
        suspicionText.alignment = TextAnchor.UpperLeft;
        suspicionText.fontStyle = FontStyle.Bold;

        reviewTitleText = CreateText("ReviewTitle", panel.transform, new Vector2(20f, -60f), new Vector2(330f, 30f), 24);
        reviewTitleText.alignment = TextAnchor.UpperLeft;
        reviewTitleText.fontStyle = FontStyle.Bold;

        reviewDescriptionText = CreateText("ReviewDescription", panel.transform, new Vector2(20f, -100f), new Vector2(330f, 70f), 20);
        reviewDescriptionText.alignment = TextAnchor.UpperLeft;
        reviewDescriptionText.horizontalOverflow = HorizontalWrapMode.Wrap;
        reviewDescriptionText.verticalOverflow = VerticalWrapMode.Truncate;
    }

    private static Text CreateText(string name, Transform parent, Vector2 anchoredPosition, Vector2 size, int fontSize)
    {
        var textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(parent, false);

        var rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPosition;

        var text = textObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.color = Color.white;
        text.text = string.Empty;
        return text;
    }

    private void UpdateDisplay(int suspicion, int reviewPoints, Review review)
    {
        if (suspicionText != null)
        {
            suspicionText.text = $"Suspicion: {suspicion}/100\nReview points: {reviewPoints}";
        }

        if (reviewTitleText != null)
        {
            reviewTitleText.text = review != null ? review.Title : "Latest review: -";
        }

        if (reviewDescriptionText != null)
        {
            reviewDescriptionText.text = review != null ? review.Description : string.Empty;
        }
    }
}
