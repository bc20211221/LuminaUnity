using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MusicButtonController : MonoBehaviour
{
    public Button musicControlButton;
    public Image buttonImage;
    public GameObject buttonContainer; // ��ť�ĸ�����������������

    void Start()
    {
        if (buttonContainer == null)
        {
            buttonContainer = gameObject;
        }

        // ��鵱ǰ�����Ƿ�����������
        string currentSceneName = SceneManager.GetActiveScene().name;
        bool isMusicScene = System.Array.IndexOf(MusicManager.Instance.musicScenes, currentSceneName) >= 0;

        // ��ʾ�����ذ�ť
        buttonContainer.SetActive(isMusicScene);

        if (isMusicScene && MusicManager.Instance != null && musicControlButton != null && buttonImage != null)
        {
            MusicManager.Instance.SetupButton(musicControlButton, buttonImage);
        }
    }
}