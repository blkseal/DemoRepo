using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.IO;
using System.Collections;
using UnityEngine.UI; // Додаємо роботу з UI

public class GameOverManager : MonoBehaviour
{
    [Header("Configurações de Vídeo e UI")]
    public VideoPlayer videoPlayer;
    public GameObject uiCanvas;
    public GameObject videoScreen;    // Об'єкт Raw Image
    public VideoClip editorVideoClip;

    [Header("Configurações de Transição de Cena")]
    public string gameplaySceneName = "DemoScene_Main";

    void Start()
    {
        if (uiCanvas != null) uiCanvas.SetActive(false);

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;

#if UNITY_EDITOR
            if (editorVideoClip != null)
            {
                videoPlayer.source = VideoSource.VideoClip;
                videoPlayer.clip = editorVideoClip;
            }
#else
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = Path.Combine(Application.streamingAssetsPath, "GameOver.mp4");
#endif

            // Налаштовуємо пряму передачу текстури у RawImage безпосередньо перед грою
            videoPlayer.sendFrameReadyEvents = true;
            videoPlayer.frameReady += OnFrameReady;

            StartCoroutine(PlayVideoRoutine());
        }
        else
        {
            OnVideoFinished(null);
        }
    }

    IEnumerator PlayVideoRoutine()
    {
        if (videoScreen != null) videoScreen.SetActive(true);

        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoPlayer.Play();
    }

    // Цей метод автоматично підхоплює кожен кадр відео і малює його на екрані!
    void OnFrameReady(VideoPlayer vp, long frameIdx)
    {
        if (videoScreen != null)
        {
            RawImage rawImage = videoScreen.GetComponent<RawImage>();
            if (rawImage != null)
            {
                rawImage.texture = vp.texture;
            }
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        if (videoPlayer != null) videoPlayer.frameReady -= OnFrameReady;
        if (videoScreen != null) videoScreen.SetActive(false);
        if (uiCanvas != null) uiCanvas.SetActive(true);
    }

    public void RestartGame()
    {
        if (!string.IsNullOrEmpty(gameplaySceneName))
        {
            SceneManager.LoadScene(gameplaySceneName);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}