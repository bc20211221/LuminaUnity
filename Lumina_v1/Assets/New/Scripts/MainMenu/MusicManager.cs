using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    public AudioSource backgroundMusicSource;
    public Sprite playSprite;
    public Sprite pauseSprite;
    public Sprite playHoverSprite;
    public Sprite pauseHoverSprite;
    public string[] musicScenes = { "Scene1", "Scene2" }; // ָ�����������ֵĳ�������
    private bool isPlaying = true;
    private bool isHovering = false;
    private bool isMusicScene = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (backgroundMusicSource == null)
        {
            backgroundMusicSource = gameObject.AddComponent<AudioSource>();
            backgroundMusicSource.loop = true;
        }

        // ע�᳡�������¼�
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        CheckCurrentScene();
    }

    private void OnDestroy()
    {
        // ȡ��ע���¼�
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckCurrentScene();
    }

    private void CheckCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        isMusicScene = System.Array.IndexOf(musicScenes, currentSceneName) >= 0;

        if (isMusicScene)
        {
            if (!backgroundMusicSource.isPlaying && backgroundMusicSource.clip != null && isPlaying)
            {
                backgroundMusicSource.Play();
            }
        }
        else
        {
            backgroundMusicSource.Pause();
        }
    }

    public void ToggleMusic()
    {
        if (!isMusicScene) return; // �����ֳ������������

        if (isPlaying)
        {
            backgroundMusicSource.Pause();
        }
        else
        {
            backgroundMusicSource.Play();
        }
        isPlaying = !isPlaying;
    }

    public void SetupButton(Button musicControlButton, Image buttonImage)
    {
        // ����ɵļ����¼�
        musicControlButton.onClick.RemoveAllListeners();
        // ����µĵ���¼�
        musicControlButton.onClick.AddListener(() => {
            ToggleMusic();
            UpdateButtonImage(buttonImage);
        });

        // �����ͣ�¼�
        EventTrigger trigger = musicControlButton.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = musicControlButton.gameObject.AddComponent<EventTrigger>();

        // ����ɵ��¼�
        trigger.triggers.Clear();

        // ���ָ������¼�
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => {
            isHovering = true;
            UpdateButtonImage(buttonImage);
        });
        trigger.triggers.Add(entryEnter);

        // ���ָ���˳��¼�
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => {
            isHovering = false;
            UpdateButtonImage(buttonImage);
        });
        trigger.triggers.Add(entryExit);

        // ��ʼ����ť״̬
        UpdateButtonImage(buttonImage);
    }

    private void UpdateButtonImage(Image buttonImage)
    {
        if (buttonImage != null && playSprite != null && pauseSprite != null)
        {
            buttonImage.sprite = isPlaying
                ? (isHovering ? pauseHoverSprite : pauseSprite)
                : (isHovering ? playHoverSprite : playSprite);
        }
    }

    // ���ñ�������
    public void SetBackgroundMusic(AudioClip clip)
    {
        if (backgroundMusicSource.clip != clip)
        {
            backgroundMusicSource.clip = clip;
            if (isMusicScene && isPlaying)
            {
                backgroundMusicSource.Play();
            }
        }
    }
}