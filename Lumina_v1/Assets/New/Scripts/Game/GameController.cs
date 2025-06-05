using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private LevelCompleteManager levelCompleteManager;

    private void Start()
    {
        // 初始化游戏
    }

    public void CheckVictoryCondition()
    {
        // 检查游戏胜利条件（示例：消灭所有敌人）
        if (AreAllEnemiesDefeated())
        {
            // 显示胜利面板
            if (levelCompleteManager != null)
                levelCompleteManager.ShowVictoryPanel();
            int currentLevelIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            int nextLevel = currentLevelIndex + 1;
            PlayerPrefs.SetInt("UnlockedLevel", Mathf.Max(PlayerPrefs.GetInt("UnlockedLevel", 1), nextLevel));
            PlayerPrefs.Save();
        }
    }

    private bool AreAllEnemiesDefeated()
    {
        // 实现你的胜利条件检测逻辑
        // 例如：检查场景中是否还有敌人
        //return GameObject.FindGameObjectsWithTag("Enemy").Length == 0;
        return false;
    }
}