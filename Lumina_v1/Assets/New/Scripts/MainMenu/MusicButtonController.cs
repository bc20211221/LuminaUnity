using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MusicButtonController : MonoBehaviour
{
    public Button musicControlButton;
    public Image buttonImage;
    public GameObject buttonContainer; // 按钮的父对象，用于整体隐藏

    void Start()
    {
        if (buttonContainer == null)
        {
            buttonContainer = gameObject;
        }

        // 检查当前场景是否允许播放音乐
        string currentSceneName = SceneManager.GetActiveScene().name;
        bool isMusicScene = System.Array.IndexOf(MusicManager.Instance.musicScenes, currentSceneName) >= 0;

        // 显示或隐藏按钮
        buttonContainer.SetActive(isMusicScene);

        if (isMusicScene && MusicManager.Instance != null && musicControlButton != null && buttonImage != null)
        {
            MusicManager.Instance.SetupButton(musicControlButton, buttonImage);
        }
    }
}