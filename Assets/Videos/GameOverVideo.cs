using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverFallback : MonoBehaviour
{
    public string nextSceneName = "GameOverScene";
    public float delay = 3f; // tempo antes de mudar de cena

    void Start()
    {
        // Mostrar o cursor e desbloqueá-lo
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Carregar a cena após o tempo definido
        Invoke("LoadGameOver", delay);
    }

    void LoadGameOver()
    {
        // Garantir novamente que o cursor está visível antes de carregar a cena
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene(nextSceneName);
    }
}