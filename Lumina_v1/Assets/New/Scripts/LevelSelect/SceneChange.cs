using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [Header("����")]
    [SerializeField] private string targetSceneName = "SceneName"; // Ŀ�곡������
    [SerializeField] private float delaySeconds = 2f;            // �ӳ�ʱ�䣨�룩

    [Header("UIԪ��")]
    [SerializeField] private Text countdownText;                 // ����ʱ��ʾ�ı�

    private Button button;
    private float countdownTimer = 0f;
    private bool isCountingDown = false;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(StartCountdown);

        // ��ʼ������ʱ�ı�
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isCountingDown)
        {
            countdownTimer -= Time.deltaTime;

            // ���µ���ʱ��ʾ
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

        // ���ð�ť��ֹ�ظ����
        button.interactable = false;

        // ��ʾ����ʱ�ı�
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = $"Loading: {countdownTimer:F1}s";
        }
    }

    private void LoadTargetScene()
    {
        // ��鳡���Ƿ����
        if (SceneManager.GetSceneByName(targetSceneName).IsValid() ||
            SceneUtility.GetBuildIndexByScenePath(targetSceneName) != -1)
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError($"Scene '{targetSceneName}' not found! " +
                          "Make sure it's added to the Build Settings.");

            // �ָ���ť״̬
            button.interactable = true;
            if (countdownText != null)
                countdownText.gameObject.SetActive(false);
        }
    }
}