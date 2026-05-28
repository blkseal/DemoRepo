using UnityEngine;
using UnityEngine.SceneManagement;

public class SuspicionGameOverWatcher : MonoBehaviour
{
    [SerializeField] private int suspicionLimit = 100;
    [SerializeField] private string gameOverSceneName = "GameOverScene";

    private bool gameOverTriggered = false;

    private void Update()
    {
        if (gameOverTriggered) return;
        if (GameStatusSystem.Instance == null) return;

        if (GameStatusSystem.Instance.SuspicionLevel >= suspicionLimit)
        {
            gameOverTriggered = true;
            SceneManager.LoadScene(gameOverSceneName);
        }
    }
}