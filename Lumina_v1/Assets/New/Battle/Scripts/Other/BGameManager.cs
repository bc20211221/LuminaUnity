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
            DontDestroyOnLoad(gameObject); // 防止被销毁
        }
        else
        {
            Destroy(gameObject); // 销毁多余的副本
        }
    }

}