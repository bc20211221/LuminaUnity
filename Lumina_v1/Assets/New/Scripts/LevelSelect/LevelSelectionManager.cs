using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionManager : MonoBehaviour
{
    [Header("�ؿ�����")]
    [SerializeField] private LevelCard[] levelCards;
    [Header("��������")]
    [SerializeField] private string[] requiredLevelScenes; // ��Ӧ�ؿ�����������ɵĳ�����

    private LevelCard activeCard = null;
    private int currentUnlockedLevel = 1;

    private void Start()
    {
        LoadUnlockedLevels();
        InitializeLevelCards();
    }

    private void LoadUnlockedLevels()
    {
        currentUnlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        Debug.Log($"��PlayerPrefs���ؽ���״̬: {currentUnlockedLevel}");
    }

    private void InitializeLevelCards()
    {
        for (int i = 0; i < levelCards.Length; i++)
        {
            bool isUnlocked = i < currentUnlockedLevel;
            levelCards[i].Initialize(this, i + 1, isUnlocked);
        }
    }

    public void OnLevelCardSelected(LevelCard card)
    {
        if (!card.IsUnlocked) return; // ����״̬����ѡ��

        if (activeCard == card)
        {
            activeCard.Collapse();
            activeCard = null;
            return;
        }

        if (activeCard != null)
        {
            activeCard.Collapse();
        }

        card.Expand();
        activeCard = card;
    }

    public void OnLevelStartRequested(int levelIndex)
    {
        if (levelIndex < requiredLevelScenes.Length)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(requiredLevelScenes[levelIndex]);
        }
    }
}

[System.Serializable]
public class LevelCard
{
    [Header("UI����")]
    public GameObject selectionView;
    public GameObject detailView;
    public Button selectionButton;
    public GameObject lockOverlay; // ����������״̬����ͼ
    public Button startButton;     // ��������ʼ��Ϸ��ť

    [Header("�ؿ�����")]
    public int levelIndex;
    public bool IsUnlocked { get; private set; }

    private LevelSelectionManager manager;
    private bool isExpanded = false;

    public void Initialize(LevelSelectionManager manager, int levelIndex, bool isUnlocked)
    {
        this.manager = manager;
        this.levelIndex = levelIndex;
        this.IsUnlocked = isUnlocked;

        // ��������״̬
        UpdateLockState();

        // ע�ᰴť�¼�
        selectionButton.onClick.AddListener(() => manager.OnLevelCardSelected(this));
        if (startButton != null)
        {
            startButton.onClick.AddListener(() => manager.OnLevelStartRequested(levelIndex));
        }

        // ��ʼ����ͼ״̬
        detailView.SetActive(false);
    }

    public void DebugPrintUnlockedLevel()
    {
        Debug.Log($"PlayerPrefs - UnlockedLevel: {PlayerPrefs.GetInt("UnlockedLevel", 1)}");
    }

    private void UpdateLockState()
    {
        if (lockOverlay != null)
        {
            lockOverlay.SetActive(!IsUnlocked);
        }

        selectionButton.interactable = IsUnlocked;
        if (startButton != null)
        {
            startButton.interactable = IsUnlocked;
        }
    }

    public void Expand()
    {
        if (!IsUnlocked) return; // ����״̬����չ��

        isExpanded = true;
        selectionView.SetActive(false);
        detailView.SetActive(true);
    }

    public void Collapse()
    {
        isExpanded = false;
        selectionView.SetActive(true);
        detailView.SetActive(false);
    }
}