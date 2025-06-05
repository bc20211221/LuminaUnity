using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;    // ��ͣ�˵�UI���
    public Button resumeButton;       // ������ť
    public Button exitButton;         // �˳���ť

    private bool isPaused = false;    // ��ͣ״̬��־

    private void Start()
    {
        // ȷ����Ϸ��ʼʱ��ͣ�˵��ǹرյ�
        pauseMenuUI.SetActive(false);

        // ���ð�ť�¼�
        resumeButton.onClick.AddListener(ResumeGame);
        exitButton.onClick.AddListener(ExitGame);
    }

    private void Update()
    {
        // ����ESC��
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
        Time.timeScale = 0f;          // ֹͣ��Ϸʱ��
        pauseMenuUI.SetActive(true);  // ��ʾ��ͣ�˵�
        Cursor.lockState = CursorLockMode.None; // �������
        Cursor.visible = true;        // ��ʾ���
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;          // �ָ���Ϸʱ��
        pauseMenuUI.SetActive(false); // ������ͣ�˵�
        Cursor.lockState = CursorLockMode.Locked; // �������
        Cursor.visible = false;       // �������
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