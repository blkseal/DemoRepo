using UnityEngine;
using UnityEngine.SceneManagement;

// Handles cursor state and auto-starting/resetting the game when the gameplay scene loads.
public class SceneLoadHandler : MonoBehaviour
{
    // Name of the main gameplay scene to detect
    [SerializeField] private string gameplaySceneName = "DemoScene_Main";

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If we've loaded the gameplay scene, ensure cursor is locked and start/reset the run
        if (scene.name == gameplaySceneName)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            var gm = GameManager.Instance;
            if (gm != null)
            {
                // StartGame resets run data and sets game state
                gm.StartGame();
            }
        }
        else
        {
            // For non-gameplay scenes (menus, results, gameover), show the cursor for UI interaction
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
