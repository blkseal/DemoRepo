using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

// Plays a video (if present) and transitions to the next scene after video ends.
// If no VideoPlayer is present, it waits a fallback delay and then loads.
public class GameOverVideoController : MonoBehaviour
{
    [Tooltip("Scene to load after the video finishes or fallback delay.")]
    [SerializeField] private string nextSceneName = "ResultScene";

    [Tooltip("Fallback delay in seconds if no VideoPlayer or if you prefer a short delay after the video.")]
    [SerializeField] private float fallbackDelay = 4f;

    private VideoPlayer videoPlayer;

    private void Awake()
    {
        // Try to find a VideoPlayer on this GameObject or anywhere in the scene
        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer == null)
        {
            videoPlayer = Object.FindObjectOfType<VideoPlayer>();
        }
    }

    private void OnEnable()
    {
        if (videoPlayer != null)
        {
            // If video already finished, just start fallback
            if (videoPlayer.isPrepared && videoPlayer.frame >= (long)videoPlayer.frameCount - 1)
            {
                StartCoroutine(WaitAndLoad(fallbackDelay));
                return;
            }

            videoPlayer.loopPointReached += OnVideoFinished;

            // If not playing, start playback
            if (!videoPlayer.isPlaying)
            {
                videoPlayer.Play();
            }
        }
        else
        {
            // No video, just wait the fallback delay
            StartCoroutine(WaitAndLoad(fallbackDelay));
        }
    }

    private void OnDisable()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        StartCoroutine(WaitAndLoad(fallbackDelay));
    }

    private IEnumerator WaitAndLoad(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Ensure cursor is unlocked and visible before leaving
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Also ensure game is unpaused / trigger game over via GameManager
        var gm = GameManager.Instance;
        if (gm != null)
        {
            gm.GameOver();
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
