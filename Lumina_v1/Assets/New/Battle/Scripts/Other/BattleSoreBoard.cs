using UnityEngine;
using UnityEngine.UI;

public class BattleScoreBoard : MonoBehaviour
{
    public Text FinallScoreText; // 在Inspector中分配UI Text组件
    public Text CompleteScoreText;
    private int battleScore = 0;
    private int musicScore = 0;

    // 单例模式确保只有一个计分板实例
    private static BattleScoreBoard instance;
    public static BattleScoreBoard Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<BattleScoreBoard>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("BattleScoreBoard");
                    instance = obj.AddComponent<BattleScoreBoard>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        // 确保单例唯一性
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        // 可选：如果希望计分板在场景切换时不被销毁
        // DontDestroyOnLoad(gameObject);
    }

    // 增加战斗分数
    public void AddScore(int points)
    {
        battleScore += points;
        UpdateFinalScoreDisplay();
        //Debug.Log($"战斗分数更新: {battleScore}, 总分: {GetFinalScore()}");
    }

    // 设置/更新音乐分数
    public void AddMusicScore(int points)
    {
        musicScore = points; // 直接替换音乐分数
        UpdateFinalScoreDisplay();
       // Debug.Log($"音乐分数更新: {musicScore}, 总分: {GetFinalScore()}");
    }

    // 获取战斗分数
    public int GetBattleScore()
    {
        return battleScore;
    }

    // 获取音乐分数
    public int GetMusicScore()
    {
        return musicScore;
    }

    // 计算最终分数
    public int GetFinalScore()
    {
        return battleScore + musicScore;
    }

    // 更新UI显示
    private void UpdateFinalScoreDisplay()
    {
        if (FinallScoreText != null)
        {
            FinallScoreText.text = $"{GetFinalScore()}";
            CompleteScoreText.text = FinallScoreText.text;
        }
    }

    // 重置所有分数
    public void ResetAllScores()
    {
        battleScore = 0;
        musicScore = 0;
        UpdateFinalScoreDisplay();
    }
}