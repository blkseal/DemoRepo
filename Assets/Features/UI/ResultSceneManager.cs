using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultSceneManager : MonoBehaviour
{
    private void Awake()
    {
        // Ler o resultado guardado
        int reviewPoints = PlayerPrefs.GetInt("FinalReview", 0);
        Debug.Log("ReviewPoints lidos: " + reviewPoints);

        // Criar Canvas se não existir
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            var canvasObj = new GameObject("ResultCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            Debug.Log("Canvas criado!");
        }
        else
        {
            Debug.Log("Canvas já existe!");
        }

        // Criar Text para resultado
        var textObj = new GameObject("ResultText", typeof(Text));
        textObj.transform.SetParent(canvas.transform, false);
        
        var resultText = textObj.GetComponent<Text>();
        resultText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        resultText.fontSize = 40;
        resultText.fontStyle = FontStyle.Bold;
        resultText.alignment = TextAnchor.MiddleCenter;

        // Criar texto com ReviewPoints
        string resultMessage = $"Pontuação: {reviewPoints}\n\n";
        
        // Mostrar texto conforme o resultado
        if (reviewPoints < 0)
        {
            resultMessage += "Estás num bom caminho para ser despedido...";
            resultText.color = Color.green;
        }
        else if (reviewPoints > 0)
        {
            resultMessage += "Infelizmente estás a fazer um bom trabalho e o Gerente está a considerar promover-te";
            resultText.color = Color.red;
        }
        else
        {
            resultMessage += "Performance neutra. O Gerente está indeciso sobre o teu futuro.";
            resultText.color = Color.yellow;
        }
        
        resultText.text = resultMessage;
        Debug.Log("Texto definido: " + resultMessage);

        var textRect = textObj.GetComponent<RectTransform>();
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(800, 200);

        // Encontrar o Button na scene
        Button backButton = FindFirstObjectByType<Button>();
        if (backButton != null)
        {
            backButton.onClick.AddListener(BackToMainMenu);
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayAgain()
    {
        // Load the main gameplay scene and let SceneLoadHandler / GameManager.StartGame handle reset
        SceneManager.LoadScene("DemoScene_Main");
    }
}
