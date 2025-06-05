using UnityEngine;
using UnityEngine.UI;

public class BattleScoreBoard : MonoBehaviour
{
    public Text FinallScoreText; // ��Inspector�з���UI Text���
    public Text CompleteScoreText;
    private int battleScore = 0;
    private int musicScore = 0;

    // ����ģʽȷ��ֻ��һ���Ʒְ�ʵ��
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
        // ȷ������Ψһ��
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        // ��ѡ�����ϣ���Ʒְ��ڳ����л�ʱ��������
        // DontDestroyOnLoad(gameObject);
    }

    // ����ս������
    public void AddScore(int points)
    {
        battleScore += points;
        UpdateFinalScoreDisplay();
        //Debug.Log($"ս����������: {battleScore}, �ܷ�: {GetFinalScore()}");
    }

    // ����/�������ַ���
    public void AddMusicScore(int points)
    {
        musicScore = points; // ֱ���滻���ַ���
        UpdateFinalScoreDisplay();
       // Debug.Log($"���ַ�������: {musicScore}, �ܷ�: {GetFinalScore()}");
    }

    // ��ȡս������
    public int GetBattleScore()
    {
        return battleScore;
    }

    // ��ȡ���ַ���
    public int GetMusicScore()
    {
        return musicScore;
    }

    // �������շ���
    public int GetFinalScore()
    {
        return battleScore + musicScore;
    }

    // ����UI��ʾ
    private void UpdateFinalScoreDisplay()
    {
        if (FinallScoreText != null)
        {
            FinallScoreText.text = $"{GetFinalScore()}";
            CompleteScoreText.text = FinallScoreText.text;
        }
    }

    // �������з���
    public void ResetAllScores()
    {
        battleScore = 0;
        musicScore = 0;
        UpdateFinalScoreDisplay();
    }
}