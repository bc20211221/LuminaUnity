using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Menu;
using NoteEditor.DTO;
using NoteEditor.Notes;

namespace Game.MusicSelect
{
    public class AutoMusicSelector : MonoBehaviour
    {
        [SerializeField] private Text pathInfo, musicInfo, fileInfo;
        [SerializeField] private Text bpm, time, notes;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Button music01Button;
        [SerializeField] private Button startButton, autoButton;

        private bool notesLoaded = false, musicLoaded = false;

        void Awake()
        {
            // 绑定按钮事件
            if (music01Button != null)
            {
                music01Button.onClick.AddListener(() => LoadInternalMusic("Music01"));
            }

            startButton.onClick.AddListener(() => StartGame(false));
            startButton.interactable = false;

            autoButton.onClick.AddListener(() => StartGame(true));
            autoButton.interactable = false;
        }

        void Update()
        {
            // 更新按钮状态
            startButton.interactable = notesLoaded && musicLoaded;
            autoButton.interactable = notesLoaded && musicLoaded;

            // 退出功能
            if (Input.GetKeyDown(KeyCode.Escape))
                Quit();
        }

        /// <summary>
        /// 从Unity内部资源加载音乐和谱面
        /// </summary>
        public void LoadInternalMusic(string musicName)
        {
            StartCoroutine(LoadInternalAssets(musicName));
        }

        private IEnumerator LoadInternalAssets(string musicName)
        {
            ShowLoadInfo(pathInfo, $"Loading {musicName}...");

            // 加载JSON文件
            TextAsset jsonAsset = Resources.Load<TextAsset>($"Notes/{musicName}");
            if (jsonAsset == null)
            {
                ShowLoadInfo(fileInfo, "JSON file not found!");
                yield break;
            }

            // 解析JSON数据
            LoadMusicInfoFromJson(jsonAsset.text);

            // 加载音乐文件
            AudioClip musicClip = Resources.Load<AudioClip>($"Music/{musicName}");
            if (musicClip == null)
            {
                ShowLoadInfo(musicInfo, "Music file not found!");
                yield break;
            }

            // 设置音乐信息
            time.text = "Time\t\t" + FormatTime((int)musicClip.length);
            audioSource.clip = musicClip;
            audioSource.Play();

            NotesContainer.Instance.music = musicClip;
            musicLoaded = true;

            ShowLoadInfo(musicInfo, "Music loaded.");

            // 等待两个资源都加载完成
            yield return new WaitUntil(() => notesLoaded && musicLoaded);

            // 自动开始游戏（可选）
            // StartGame(false);
        }

        /// <summary>
        /// 根据JSON数据加载音乐信息
        /// </summary>
        private void LoadMusicInfoFromJson(string json)
        {
            var editData = JsonUtility.FromJson<MusicDTO.EditData>(json);

            if (editData == null)
            {
                ShowLoadInfo(fileInfo, "Invalid JSON file!");
                return;
            }

            ShowLoadInfo(fileInfo, "JSON loaded!");
            notesLoaded = true;

            bpm.text = "BPM\t\t" + editData.BPM;
            notes.text = "Notes\t" + editData.notes.Count;

            NotesContainer.Instance.json = json;
        }

        public void StartGame(bool autoplay)
        {
            if (notesLoaded && musicLoaded)
            {
                NotesContainer.Instance.autoplay = autoplay;
                SceneManager.LoadScene("Game");
            }
            else
            {
                Debug.LogError("Assets not fully loaded!");
            }
        }

        public void Quit()
        {
            SceneManager.LoadScene("MainMenu");
        }

        private void ShowLoadInfo(Text text, string info)
        {
            text.text = info;
        }

        // 简化的时间格式化方法
        private string FormatTime(int seconds)
        {
            return $"{seconds / 60:00}:{seconds % 60:00}";
        }
    }
}