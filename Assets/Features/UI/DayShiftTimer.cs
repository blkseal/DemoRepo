using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DayShiftTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("Total real-time duration of the shift in seconds.")]
    public float startTime = 10 * 60f; // 10 minutes

    [Tooltip("If set to 0 or greater this value will override the runtime currentTime for testing (seconds). Set -1 to use startTime).")]
    [SerializeField] private float initialCurrentTime = -1f;

    private float currentTime;

    private Text timerText;
    private Text shiftText;

    private bool flashing = false;

    private void Awake()
    {
        currentTime = (initialCurrentTime >= 0f) ? initialCurrentTime : startTime;
        CreateUI();
    }

    private void Update()
    {
        currentTime -= Time.deltaTime;

        if (currentTime < 0)
        {
            currentTime = 0;
            EndDay();
        }

        UpdateTimerDisplay();

        // Flashing handled inside UpdateTimerDisplay based on display time
    }

    private void CreateUI()
    {
        // Canvas
        var canvas = new GameObject("DayShiftCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.GetComponent<Canvas>().sortingOrder = 999;
        // Do not persist the timer canvas across scenes; let it be destroyed with the scene
        // DontDestroyOnLoad(canvas);

        // SHIFT LABEL
        var shiftObj = new GameObject("ShiftLabel", typeof(Text));
        shiftObj.transform.SetParent(canvas.transform, false);

        shiftText = shiftObj.GetComponent<Text>();
        shiftText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        shiftText.fontSize = 32;
        shiftText.fontStyle = FontStyle.Bold;
        shiftText.color = Color.white;
        shiftText.text = "day shift";
        shiftText.alignment = TextAnchor.MiddleCenter;

        var shiftRect = shiftText.GetComponent<RectTransform>();
        shiftRect.anchorMin = new Vector2(1f, 1f);
        shiftRect.anchorMax = new Vector2(1f, 1f);
        shiftRect.pivot = new Vector2(1f, 1f);
        shiftRect.anchoredPosition = new Vector2(-20f, -20f);
        shiftRect.sizeDelta = new Vector2(200f, 50f);

        // TIMER LABEL
        var timerObj = new GameObject("TimerLabel", typeof(Text));
        timerObj.transform.SetParent(canvas.transform, false);

        timerText = timerObj.GetComponent<Text>();
        timerText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        timerText.fontSize = 48;
        timerText.fontStyle = FontStyle.Bold;
        timerText.color = Color.white;
        timerText.alignment = TextAnchor.MiddleCenter;

        var timerRect = timerText.GetComponent<RectTransform>();
        timerRect.anchorMin = new Vector2(1f, 1f);
        timerRect.anchorMax = new Vector2(1f, 1f);
        timerRect.pivot = new Vector2(1f, 1f);
        timerRect.anchoredPosition = new Vector2(-20f, -60f);
        timerRect.sizeDelta = new Vector2(200f, 60f);
    }

    private void UpdateTimerDisplay()
    {
        // elapsed real seconds since shift start
        float elapsedSeconds = startTime - currentTime;
        elapsedSeconds = Mathf.Clamp(elapsedSeconds, 0f, startTime);

        // Map elapsedSeconds (0..startTime) to display minutes from 0..180 (12:00 -> 15:00 = 3 hours)
        float totalDisplayMinutes = 3f * 60f; // 180 minutes
        float fraction = (startTime > 0f) ? (elapsedSeconds / startTime) : 1f;
        fraction = Mathf.Clamp01(fraction);
        float displayMinutesFloat = fraction * totalDisplayMinutes;
        int minutosReais = Mathf.FloorToInt(displayMinutesFloat);

        // Hora início: 12:00
        int horaAtual = 12 + (minutosReais / 60);
        int minutoAtual = minutosReais % 60;

        // Cap at 15:00
        if (horaAtual > 15 || (horaAtual == 15 && minutoAtual > 0))
        {
            horaAtual = 15;
            minutoAtual = 0;
        }

        timerText.text = $"{horaAtual:00}:{minutoAtual:00}";

        // Start flashing from 14:45 to 15:00
        bool shouldFlash = (horaAtual > 14) || (horaAtual == 14 && minutoAtual >= 45);

        if (shouldFlash && !flashing)
        {
            flashing = true;
            InvokeRepeating(nameof(FlashRed), 0f, 0.5f);
        }
        else if (!shouldFlash && flashing)
        {
            flashing = false;
            CancelInvoke(nameof(FlashRed));
            timerText.color = Color.white;
        }
    }

    private void FlashRed()
    {
        timerText.color = timerText.color == Color.white ? Color.red : Color.white;
    }

    private void EndDay()
    {
        int suspicion = GameStatusSystem.Instance.SuspicionLevel;
        int review = GameStatusSystem.Instance.ReviewPoints;

        PlayerPrefs.SetInt("FinalSuspicion", suspicion);
        PlayerPrefs.SetInt("FinalReview", review);

        // Destruir o Canvas do timer
        GameObject timerCanvas = GameObject.Find("DayShiftCanvas");
        if (timerCanvas != null)
        {
            Destroy(timerCanvas);
        }

        // Use EndOfDayCoordinator to ensure the gerente event always runs before showing results
        var coordinator = Object.FindObjectOfType<EndOfDayCoordinator>();
        if (coordinator != null)
        {
            coordinator.HandleEndOfDay();
        }
        else
        {
            // Fallback to loading the result scene directly
            SceneManager.LoadScene("ResultScene");
        }
    }
}
