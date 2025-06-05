using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialSystem : MonoBehaviour
{
    [Header("教程设置")]
    [SerializeField] private string hasSeenTutorialKey = "HasSeenTutorial";
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("UI引用")]
    [SerializeField] private GameObject tutorialUI;
    [SerializeField] private GameObject prevButton;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject startButton;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private GameObject[] tutorialSteps;

    private int currentStep = 0;

    private void Start()
    {
        // 检查玩家是否已经看过教程
        if (PlayerPrefs.HasKey(hasSeenTutorialKey) &&
            PlayerPrefs.GetInt(hasSeenTutorialKey) == 1)
        {
            LoadGameScene();
            return;
        }

        // 首次进入，显示教程UI
        ShowTutorial();
    }

    private void ShowTutorial()
    {
        // 激活教程UI并初始化第一步
        tutorialUI.SetActive(true);
        currentStep = 0;
        UpdateTutorialStep();

        // 设置按钮事件
        prevButton.GetComponent<Button>().onClick.AddListener(GoToPreviousStep);
        nextButton.GetComponent<Button>().onClick.AddListener(GoToNextStep);
        startButton.GetComponent<Button>().onClick.AddListener(FinishTutorial);
        nextButton.SetActive(true);
    }

    private void GoToPreviousStep()
    {
        if (currentStep > 0)
        {
            currentStep--;
            UpdateTutorialStep();
        }
    }

    private void GoToNextStep()
    {
        if (currentStep < tutorialSteps.Length - 1)
        {
            currentStep++;
            UpdateTutorialStep();
        }
    }

    private void FinishTutorial()
    {
        // 完成教程，保存状态并加载游戏场景
        PlayerPrefs.SetInt(hasSeenTutorialKey, 1);
        PlayerPrefs.Save();
        LoadGameScene();
    }

    private void UpdateTutorialStep()
    {
        // 隐藏所有步骤
        foreach (var step in tutorialSteps)
        {
            step.SetActive(false);
        }

        // 显示当前步骤
        tutorialSteps[currentStep].SetActive(true);

        // 更新进度条
        if (progressSlider != null)
        {
            progressSlider.value = (float)currentStep / (tutorialSteps.Length - 1);
        }

        // 更新按钮显示状态
        prevButton.SetActive(currentStep > 0);
        nextButton.SetActive(currentStep==0);
        startButton.SetActive(currentStep == tutorialSteps.Length - 1);
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}