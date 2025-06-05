using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private LevelCompleteManager levelCompleteManager;

    private void Start()
    {
        // ��ʼ����Ϸ
    }

    public void CheckVictoryCondition()
    {
        // �����Ϸʤ��������ʾ�����������е��ˣ�
        if (AreAllEnemiesDefeated())
        {
            // ��ʾʤ�����
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
        // ʵ�����ʤ����������߼�
        // ���磺��鳡�����Ƿ��е���
        //return GameObject.FindGameObjectsWithTag("Enemy").Length == 0;
        return false;
    }
}