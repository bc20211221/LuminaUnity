using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionManager : MonoBehaviour
{
    [Header("关卡配置")]
    [SerializeField] private LevelCard[] levelCards;
    [Header("解锁条件")]
    [SerializeField] private string[] requiredLevelScenes; // 对应关卡解锁所需完成的场景名

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
        Debug.Log($"从PlayerPrefs加载解锁状态: {currentUnlockedLevel}");
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
        if (!card.IsUnlocked) return; // 锁定状态不可选择

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
    [Header("UI引用")]
    public GameObject selectionView;
    public GameObject detailView;
    public Button selectionButton;
    public GameObject lockOverlay; // 新增：锁定状态覆盖图
    public Button startButton;     // 新增：开始游戏按钮

    [Header("关卡数据")]
    public int levelIndex;
    public bool IsUnlocked { get; private set; }

    private LevelSelectionManager manager;
    private bool isExpanded = false;

    public void Initialize(LevelSelectionManager manager, int levelIndex, bool isUnlocked)
    {
        this.manager = manager;
        this.levelIndex = levelIndex;
        this.IsUnlocked = isUnlocked;

        // 设置锁定状态
        UpdateLockState();

        // 注册按钮事件
        selectionButton.onClick.AddListener(() => manager.OnLevelCardSelected(this));
        if (startButton != null)
        {
            startButton.onClick.AddListener(() => manager.OnLevelStartRequested(levelIndex));
        }

        // 初始化视图状态
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
        if (!IsUnlocked) return; // 锁定状态不可展开

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