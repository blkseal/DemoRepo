using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GameOverVideoManager : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "GameOverScene";

    private VideoPlayer videoPlayer;

    private void Start()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;

        videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer == null)
        {
            Debug.LogError("На цьому об'єкті немає VideoPlayer.");
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Play();
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(nextSceneName);
    }
}