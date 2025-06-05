using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;    // 暂停菜单UI面板
    public Button resumeButton;       // 继续按钮
    public Button exitButton;         // 退出按钮

    private bool isPaused = false;    // 暂停状态标志

    private void Start()
    {
        // 确保游戏开始时暂停菜单是关闭的
        pauseMenuUI.SetActive(false);

        // 设置按钮事件
        resumeButton.onClick.AddListener(ResumeGame);
        exitButton.onClick.AddListener(ExitGame);
    }

    private void Update()
    {
        // 监听ESC键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;          // 停止游戏时间
        pauseMenuUI.SetActive(true);  // 显示暂停菜单
        Cursor.lockState = CursorLockMode.None; // 解锁鼠标
        Cursor.visible = true;        // 显示鼠标
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;          // 恢复游戏时间
        pauseMenuUI.SetActive(false); // 隐藏暂停菜单
        Cursor.lockState = CursorLockMode.Locked; // 锁定鼠标
        Cursor.visible = false;       // 隐藏鼠标
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}