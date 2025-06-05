using UnityEngine;
using UnityEngine.UI;

public class AudioButtonController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private Button playButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private bool playOnAwake = false;
    [SerializeField] private bool loopMusic = false;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = musicClip;
        audioSource.loop = loopMusic;

        // ��ʼ����ť״̬
        UpdateButtonVisibility();

        // ���ð�ť�¼�
        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayButtonClick);
        }

        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(OnPauseButtonClick);
        }

        // ����ʱ�Զ�����
        if (playOnAwake && musicClip != null)
        {
            PlayMusic();
        }
    }

    private void UpdateButtonVisibility()
    {
        bool isPlaying = audioSource.isPlaying;
        playButton.gameObject.SetActive(!isPlaying);
        pauseButton.gameObject.SetActive(isPlaying);
    }

    public void OnPlayButtonClick()
    {
        PlayMusic();
        UpdateButtonVisibility();
    }

    public void OnPauseButtonClick()
    {
        PauseMusic();
        UpdateButtonVisibility();
    }

    public void PlayMusic()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = musicClip;
            audioSource.Play();
        }
    }

    public void PauseMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }
}