using UnityEngine;
using UnityEngine.SceneManagement;

// Toggle the management menu scene with Q or Escape and pause/resume the game.
public class ManagementMenuToggle : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "MenuGestaoScene";

    private bool menuLoaded = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        if (!menuLoaded)
        {
            // lock player interactions so gameplay input stops
            PlayerInteractionState.SetLocked(true);

            // load additively so the menu overlays the current scene
            SceneManager.LoadSceneAsync(menuSceneName, LoadSceneMode.Additive);

            // Pause via GameManager if available, fallback to Time.timeScale
            var gm = GameManager.Instance;
            if (gm != null)
            {
                gm.PauseGame();
            }
            else
            {
                Time.timeScale = 0f;
            }

            // Ensure cursor is visible for menu interaction
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            menuLoaded = true;
        }
        else
        {
            var scene = SceneManager.GetSceneByName(menuSceneName);
            if (scene.IsValid() && scene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(scene);
            }

            var gm = GameManager.Instance;
            if (gm != null)
            {
                gm.ResumeGame();
            }
            else
            {
                Time.timeScale = 1f;
            }

            // Return cursor and interaction state to gameplay
            PlayerInteractionState.SetLocked(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            menuLoaded = false;
        }
    }
}
