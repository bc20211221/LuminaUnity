using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using UnityEngine.UI; // ← 加载 UI 命名空间
using NoteEditor.DTO;
using Game.MusicSelect;

namespace Game.Load
{
    public class LoadManager : MonoBehaviour
    {
        [Header("文件名（相对 StreamingAssets）")]
        public string fileBaseName = "AutoLoad/music1";

        [Header("目标场景名")]
        public string nextSceneName = "GameScene";

        [Header("控制跳转的按钮（加载完成后启用）")]
        public Button enterButton;

        private bool isLoaded = false;

        void Start()
        {
            if (enterButton != null)
                enterButton.gameObject.SetActive(false); // 初始隐藏按钮

            StartCoroutine(LoadDataCoroutine());
        }

        IEnumerator LoadDataCoroutine()
        {
            string jsonPath = Path.Combine(Application.streamingAssetsPath, fileBaseName + ".json");
            string musicPath = Path.Combine(Application.streamingAssetsPath, fileBaseName + ".wav");

            // 加载 JSON
            string json;
#if UNITY_ANDROID && !UNITY_EDITOR
            UnityWebRequest jsonRequest = UnityWebRequest.Get(jsonPath);
            yield return jsonRequest.SendWebRequest();
            json = jsonRequest.downloadHandler.text;
#else
            if (!File.Exists(jsonPath))
            {
                Debug.LogError("找不到 JSON 文件：" + jsonPath);
                yield break;
            }
            json = File.ReadAllText(jsonPath);
#endif

            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError("JSON 加载失败！");
                yield break;
            }

            NotesContainer.Instance.json = json;
            Debug.Log("JSON 加载成功");

            var editData = JsonUtility.FromJson<MusicDTO.EditData>(json);
            if (editData == null)
            {
                Debug.LogError("JSON 解析失败！");
                yield break;
            }

            // 加载音频
            UnityWebRequest musicRequest = UnityWebRequestMultimedia.GetAudioClip("file://" + musicPath, AudioType.WAV);
            yield return musicRequest.SendWebRequest();

            if (musicRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("音频加载失败：" + musicRequest.error);
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(musicRequest);
            if (clip == null)
            {
                Debug.LogError("音频无效！");
                yield break;
            }

            NotesContainer.Instance.music = clip;
            NotesContainer.Instance.autoplay = false;

            Debug.Log($"音频加载完成：{clip.length:F2}s, {clip.frequency}Hz");

            isLoaded = true;

            // 启用跳转按钮
            if (enterButton != null)
            {
                enterButton.gameObject.SetActive(true);
                Debug.Log("资源加载完毕，按钮已启用");
            }
        }

        public void EnterGame()
        {
            if (isLoaded)
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.LogWarning("资源尚未加载完，无法进入游戏！");
            }
        }

        public bool IsLoaded()
        {
            return isLoaded;
        }
    }
}
