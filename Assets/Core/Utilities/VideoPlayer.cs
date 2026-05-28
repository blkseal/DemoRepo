using UnityEngine;
using UnityEngine.Video;

public class PlayVideo : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [Tooltip("Used on WebGL; the video must be served from StreamingAssets or a web URL.")]
    [SerializeField] private string webglVideoFileName = "intro.mp4";

    private void Start()
    {
        Play();
    }

    public void Play()
    {
        if (videoPlayer == null)
        {
            Debug.LogWarning($"{name}: No VideoPlayer assigned.");
            return;
        }

        videoPlayer.playOnAwake = false;
        videoPlayer.waitForFirstFrame = true;

#if UNITY_WEBGL
        // WebGL works best with URL-based playback, typically from StreamingAssets.
        if (videoPlayer.source != VideoSource.Url)
        {
            videoPlayer.source = VideoSource.Url;
        }

        if (!string.IsNullOrWhiteSpace(webglVideoFileName))
        {
            videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, webglVideoFileName);
        }
#endif

        if (videoPlayer.isPrepared)
        {
            videoPlayer.Play();
            return;
        }

        videoPlayer.prepareCompleted -= OnPrepared;
        videoPlayer.prepareCompleted += OnPrepared;
        videoPlayer.Prepare();
    }

    private void OnPrepared(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnPrepared;
        vp.Play();
    }
}