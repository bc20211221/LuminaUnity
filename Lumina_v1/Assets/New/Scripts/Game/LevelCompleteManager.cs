using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelCompleteManager : MonoBehaviour
{
    [SerializeField] private GameObject victoryPanel; // ʤ�����
    [SerializeField] private GameObject defeatPanel; // ʤ�����

    [SerializeField] private GameObject NextLevel; // ʤ�����
    [SerializeField] private string nextLevelName;    // ��һ�س�������
    public static LevelCompleteManager Instance { get; private set; }

    private void Start()
    {
        NextLevel.SetActive(false);
        /*// ��ʼʱʤ����岻�ɼ�
        if (victoryPanel != null)
            victoryPanel.SetActive(false);
        if (defeatPanel != null)
            defeatPanel.SetActive(false);*/

        UnlockNextLevel();
    }


    public void ShowVictoryPanel()
    {
        Debug.Log("���� ShowVictoryPanel ����");
        NextLevelButton();
        Debug.Log("��ʾʤ����壬׼��������һ��");
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            
        }
        defeatPanel.SetActive(false);
        UnlockNextLevel();

    }

    public void NextLevelButton()
    {
        Debug.Log("��ʾ��һ�ذ�ť");
        if (NextLevel != null)
        {
            NextLevel.SetActive(true);
            
        }
    }

    public void UnlockNextLevel()
    {
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        int currentLevelIndex = int.Parse(currentSceneName.Replace("Level", ""));
        Debug.Log($"��ǰ�ؿ�: {currentSceneName}, �ؿ�����: {currentLevelIndex}");

        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        Debug.Log($"��ǰ�ѽ����ؿ�: {unlockedLevel}");

        if (currentLevelIndex >= unlockedLevel && currentLevelIndex < 4)
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentLevelIndex + 1);
            PlayerPrefs.Save();
            Debug.Log($"�ѽ����ؿ�: {currentLevelIndex + 1}");
        }
    }

    public void ReturnToLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }
}