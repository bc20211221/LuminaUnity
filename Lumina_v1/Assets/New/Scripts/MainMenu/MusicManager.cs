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
    public string[] musicScenes = { "Scene1", "Scene2" }; // 指定允许播放音乐的场景名称
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

        // 注册场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        CheckCurrentScene();
    }

    private void OnDestroy()
    {
        // 取消注册事件
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
        if (!isMusicScene) return; // 非音乐场景不允许操作

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
        // 清除旧的监听事件
        musicControlButton.onClick.RemoveAllListeners();
        // 添加新的点击事件
        musicControlButton.onClick.AddListener(() => {
            ToggleMusic();
            UpdateButtonImage(buttonImage);
        });

        // 添加悬停事件
        EventTrigger trigger = musicControlButton.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = musicControlButton.gameObject.AddComponent<EventTrigger>();

        // 清除旧的事件
        trigger.triggers.Clear();

        // 添加指针进入事件
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => {
            isHovering = true;
            UpdateButtonImage(buttonImage);
        });
        trigger.triggers.Add(entryEnter);

        // 添加指针退出事件
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => {
            isHovering = false;
            UpdateButtonImage(buttonImage);
        });
        trigger.triggers.Add(entryExit);

        // 初始化按钮状态
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

    // 设置背景音乐
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