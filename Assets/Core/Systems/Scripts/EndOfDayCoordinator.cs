using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Coordinates end-of-day: ensures manager event plays before loading ResultScene.
public class EndOfDayCoordinator : MonoBehaviour
{
    [Tooltip("Name of the scene to load after end of day and manager event.")]
    [SerializeField] private string resultSceneName = "ResultScene";

    [Tooltip("Safety timeout in seconds in case the manager event never finishes.")]
    [SerializeField] private float safetyTimeout = 10f;

    public void HandleEndOfDay()
    {
        StartCoroutine(HandleEndOfDayCoroutine());
    }

    private IEnumerator HandleEndOfDayCoroutine()
    {
        // Try to find the GerenteController in the scene
        var gerente = Object.FindObjectOfType<GerenteController>();

        if (gerente != null && !gerente.HasEventStarted)
        {
            bool finished = false;
            System.Action onFinished = () => finished = true;
            gerente.OnEventFinished += onFinished;

            // Force start the event immediately
            gerente.StartEventImmediate();

            float t = 0f;
            while (!finished && t < safetyTimeout)
            {
                t += Time.deltaTime;
                yield return null;
            }

            gerente.OnEventFinished -= onFinished;
        }
        else if (gerente != null && gerente.HasEventStarted)
        {
            // If already started, wait for finish
            bool finished = false;
            System.Action onFinished = () => finished = true;
            gerente.OnEventFinished += onFinished;

            float t = 0f;
            while (!finished && t < safetyTimeout)
            {
                t += Time.deltaTime;
                yield return null;
            }

            gerente.OnEventFinished -= onFinished;
        }
        else
        {
            // No gerente found — proceed immediately
            yield return null;
        }

        // After manager event finished or timeout, load result scene
        if (!string.IsNullOrEmpty(resultSceneName))
        {
            SceneManager.LoadScene(resultSceneName);
        }
    }
}
