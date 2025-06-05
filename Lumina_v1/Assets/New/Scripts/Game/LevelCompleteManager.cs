using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelCompleteManager : MonoBehaviour
{
    [SerializeField] private GameObject victoryPanel; // 胜利面板
    [SerializeField] private GameObject defeatPanel; // 胜利面板

    [SerializeField] private GameObject NextLevel; // 胜利面板
    [SerializeField] private string nextLevelName;    // 下一关场景名称
    public static LevelCompleteManager Instance { get; private set; }

    private void Start()
    {
        NextLevel.SetActive(false);
        /*// 初始时胜利面板不可见
        if (victoryPanel != null)
            victoryPanel.SetActive(false);
        if (defeatPanel != null)
            defeatPanel.SetActive(false);*/

        UnlockNextLevel();
    }


    public void ShowVictoryPanel()
    {
        Debug.Log("调用 ShowVictoryPanel 方法");
        NextLevelButton();
        Debug.Log("显示胜利面板，准备解锁下一关");
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            
        }
        defeatPanel.SetActive(false);
        UnlockNextLevel();

    }

    public void NextLevelButton()
    {
        Debug.Log("显示下一关按钮");
        if (NextLevel != null)
        {
            NextLevel.SetActive(true);
            
        }
    }

    public void UnlockNextLevel()
    {
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        int currentLevelIndex = int.Parse(currentSceneName.Replace("Level", ""));
        Debug.Log($"当前关卡: {currentSceneName}, 关卡索引: {currentLevelIndex}");

        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        Debug.Log($"当前已解锁关卡: {unlockedLevel}");

        if (currentLevelIndex >= unlockedLevel && currentLevelIndex < 4)
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentLevelIndex + 1);
            PlayerPrefs.Save();
            Debug.Log($"已解锁关卡: {currentLevelIndex + 1}");
        }
    }

    public void ReturnToLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }
}