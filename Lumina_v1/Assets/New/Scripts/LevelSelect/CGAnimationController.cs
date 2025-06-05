using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CGAnimationController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string sceneToLoadAfterCG = "MainGameScene";
    [SerializeField] private GameObject skipButton;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private string hasSeenCGKey = "HasSeenCG";
    [SerializeField] private CanvasGroup animationCanvasGroup;
    [SerializeField] private bool useFadeEffect = true;
    [SerializeField] private float fadeDuration = 0.5f;

    private bool isPlaying = false;
    private bool isFading = false;

    private void Start()
    {
        // 默认隐藏动画图层
        if (animationCanvasGroup != null)
        {
            animationCanvasGroup.alpha = 0;
            animationCanvasGroup.blocksRaycasts = false;
            animationCanvasGroup.interactable = false;
        }

        // 检查玩家是否已经看过CG
        if (PlayerPrefs.HasKey(hasSeenCGKey) && PlayerPrefs.GetInt(hasSeenCGKey) == 1)
        {
            LoadNextScene();
            return;
        }

        // 标记玩家已看过CG
        PlayerPrefs.SetInt(hasSeenCGKey, 1);
        PlayerPrefs.Save();

        // 显示动画图层并播放CG
        ShowAnimationCanvas();
    }

    private void ShowAnimationCanvas()
    {
        if (animationCanvasGroup != null)
        {
            if (useFadeEffect)
            {
                StartCoroutine(FadeInAnimationCanvas());
            }
            else
            {
                animationCanvasGroup.alpha = 1;
                animationCanvasGroup.blocksRaycasts = true;
                animationCanvasGroup.interactable = true;
                StartCG();
            }
        }
        else
        {
            StartCG();
        }
    }

    private System.Collections.IEnumerator FadeInAnimationCanvas()
    {
        animationCanvasGroup.blocksRaycasts = true;
        animationCanvasGroup.interactable = true;

        float startTime = Time.time;

        while (Time.time < startTime + fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;
            animationCanvasGroup.alpha = Mathf.Lerp(0, 1, t);
            yield return null;
        }

        animationCanvasGroup.alpha = 1;
        StartCG();
    }

    private void StartCG()
    {
        // 开始播放CG
        videoPlayer.loopPointReached += OnCGFinished;
        videoPlayer.Play();
        isPlaying = true;

        // 显示跳过按钮
        if (skipButton != null)
        {
            skipButton.SetActive(true);
        }

        // 隐藏继续按钮
        if (continueButton != null)
        {
            continueButton.SetActive(false);
        }
    }

    private void Update()
    {
        // 检测跳过输入（如按ESC键）
        if (isPlaying && !isFading && Input.GetKeyDown(KeyCode.Escape))
        {
            SkipCG();
        }
    }

    public void SkipCG()
    {
        if (!isPlaying || isFading) return;

        isFading = true;
        isPlaying = false;

        // 停止视频播放
        videoPlayer.Stop();

        // 隐藏动画图层
        HideAnimationCanvas();
    }

    private void HideAnimationCanvas()
    {
        if (animationCanvasGroup != null)
        {
            if (useFadeEffect)
            {
                StartCoroutine(FadeOutAnimationCanvas());
            }
            else
            {
                animationCanvasGroup.alpha = 0;
                animationCanvasGroup.blocksRaycasts = false;
                animationCanvasGroup.interactable = false;
                ShowContinueButton();
            }
        }
        else
        {
            ShowContinueButton();
        }
    }

    private System.Collections.IEnumerator FadeOutAnimationCanvas()
    {
        float startTime = Time.time;
        float startAlpha = animationCanvasGroup.alpha;

        while (Time.time < startTime + fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;
            animationCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t);
            yield return null;
        }

        animationCanvasGroup.alpha = 0;
        animationCanvasGroup.blocksRaycasts = false;
        animationCanvasGroup.interactable = false;

        ShowContinueButton();
        isFading = false;
    }

    private void ShowContinueButton()
    {
        if (skipButton != null)
        {
            skipButton.SetActive(false);
        }

        if (continueButton != null)
        {
            continueButton.SetActive(true);
        }
    }

    private void OnCGFinished(VideoPlayer vp)
    {
        if (isFading) return;
        Debug.Log("CG播放完毕，准备隐藏图层");
        isPlaying = false;
        
        //HideAnimationCanvas();
        ShowContinueButton();
        LoadNextScene();
    }

    public void ContinueToNextScene()
    {
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        Debug.Log($"准备加载场景: {sceneToLoadAfterCG}");
        SceneManager.LoadScene(sceneToLoadAfterCG);
    }
}