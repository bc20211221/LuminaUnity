using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [Header("配置")]
    [SerializeField] private string targetSceneName = "SceneName"; // 目标场景名称
    [SerializeField] private float delaySeconds = 2f;            // 延迟时间（秒）

    [Header("UI元素")]
    [SerializeField] private Text countdownText;                 // 倒计时显示文本

    private Button button;
    private float countdownTimer = 0f;
    private bool isCountingDown = false;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(StartCountdown);

        // 初始化倒计时文本
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isCountingDown)
        {
            countdownTimer -= Time.deltaTime;

            // 更新倒计时显示
            if (countdownText != null)
                countdownText.text = $"Loading: {countdownTimer:F1}s";

            if (countdownTimer <= 0f)
            {
                isCountingDown = false;
                LoadTargetScene();
            }
        }
    }

    private void StartCountdown()
    {
        if (isCountingDown) return;

        isCountingDown = true;
        countdownTimer = delaySeconds;

        // 禁用按钮防止重复点击
        button.interactable = false;

        // 显示倒计时文本
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = $"Loading: {countdownTimer:F1}s";
        }
    }

    private void LoadTargetScene()
    {
        // 检查场景是否存在
        if (SceneManager.GetSceneByName(targetSceneName).IsValid() ||
            SceneUtility.GetBuildIndexByScenePath(targetSceneName) != -1)
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError($"Scene '{targetSceneName}' not found! " +
                          "Make sure it's added to the Build Settings.");

            // 恢复按钮状态
            button.interactable = true;
            if (countdownText != null)
                countdownText.gameObject.SetActive(false);
        }
    }
}