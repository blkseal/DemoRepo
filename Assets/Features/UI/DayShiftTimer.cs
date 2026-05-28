using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DayShiftTimer : MonoBehaviour
{
    public float startTime = 15 * 60f; // 15 minutos
    private float currentTime;

    private Text timerText;
    private Text shiftText;

    private bool flashing = false;

    private void Awake()
    {
        currentTime = startTime;
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

        // Flashing apenas entre os 10 e 15 minutos (quando currentTime está entre 600 e 300 segundos)
        if (currentTime <= 600f && currentTime > 300f && !flashing)
        {
            flashing = true;
            InvokeRepeating(nameof(FlashRed), 0f, 0.5f);
        }

        // Parar flashing quando passa dos 15 minutos
        if (currentTime <= 300f && flashing)
        {
            flashing = false;
            CancelInvoke(nameof(FlashRed));
            timerText.color = Color.white;
        }
    }

    private void CreateUI()
    {
        // Canvas
        var canvas = new GameObject("DayShiftCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.GetComponent<Canvas>().sortingOrder = 999;
        DontDestroyOnLoad(canvas);

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
        // Tempo decorrido em segundos
        float elapsedSeconds = startTime - currentTime;
        
        // Regra: cada 4 segundos de jogo = 1 minuto real (turno de 15 min = 60 seg de jogo)
        int minutosReais = Mathf.FloorToInt(elapsedSeconds / 4f);
        
        // Hora início: 12:00
        int horaAtual = 12;
        int minutoAtual = minutosReais;
        
        // Ajustar para horas
        horaAtual += minutoAtual / 60;
        minutoAtual = minutoAtual % 60;
        
        // Garantir que não ultrapassa 15:00
        if (horaAtual > 15)
            horaAtual = 15;
        
        timerText.text = $"{horaAtual:00}:{minutoAtual:00}";
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

        SceneManager.LoadScene("ResultScene");
    }
}
