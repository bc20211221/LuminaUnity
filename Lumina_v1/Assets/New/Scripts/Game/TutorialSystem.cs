using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialSystem : MonoBehaviour
{
    [Header("�̳�����")]
    [SerializeField] private string hasSeenTutorialKey = "HasSeenTutorial";
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("UI����")]
    [SerializeField] private GameObject tutorialUI;
    [SerializeField] private GameObject prevButton;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject startButton;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private GameObject[] tutorialSteps;

    private int currentStep = 0;

    private void Start()
    {
        // �������Ƿ��Ѿ������̳�
        if (PlayerPrefs.HasKey(hasSeenTutorialKey) &&
            PlayerPrefs.GetInt(hasSeenTutorialKey) == 1)
        {
            LoadGameScene();
            return;
        }

        // �״ν��룬��ʾ�̳�UI
        ShowTutorial();
    }

    private void ShowTutorial()
    {
        // ����̳�UI����ʼ����һ��
        tutorialUI.SetActive(true);
        currentStep = 0;
        UpdateTutorialStep();

        // ���ð�ť�¼�
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
        // ��ɽ̳̣�����״̬��������Ϸ����
        PlayerPrefs.SetInt(hasSeenTutorialKey, 1);
        PlayerPrefs.Save();
        LoadGameScene();
    }

    private void UpdateTutorialStep()
    {
        // �������в���
        foreach (var step in tutorialSteps)
        {
            step.SetActive(false);
        }

        // ��ʾ��ǰ����
        tutorialSteps[currentStep].SetActive(true);

        // ���½�����
        if (progressSlider != null)
        {
            progressSlider.value = (float)currentStep / (tutorialSteps.Length - 1);
        }

        // ���°�ť��ʾ״̬
        prevButton.SetActive(currentStep > 0);
        nextButton.SetActive(currentStep==0);
        startButton.SetActive(currentStep == tutorialSteps.Length - 1);
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}