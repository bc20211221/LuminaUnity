using UnityEngine;

public class BGameManager : MonoBehaviour
{
    public static BGameManager Instance;
    public BattleArea battleArea;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ��ֹ������
        }
        else
        {
            Destroy(gameObject); // ���ٶ���ĸ���
        }
    }

}