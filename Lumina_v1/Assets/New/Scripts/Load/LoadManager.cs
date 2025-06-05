using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using UnityEngine.UI; // �� ���� UI �����ռ�
using NoteEditor.DTO;
using Game.MusicSelect;

namespace Game.Load
{
    public class LoadManager : MonoBehaviour
    {
        [Header("�ļ�������� StreamingAssets��")]
        public string fileBaseName = "AutoLoad/music1";

        [Header("Ŀ�곡����")]
        public string nextSceneName = "GameScene";

        [Header("������ת�İ�ť��������ɺ����ã�")]
        public Button enterButton;

        private bool isLoaded = false;

        void Start()
        {
            if (enterButton != null)
                enterButton.gameObject.SetActive(false); // ��ʼ���ذ�ť

            StartCoroutine(LoadDataCoroutine());
        }

        IEnumerator LoadDataCoroutine()
        {
            string jsonPath = Path.Combine(Application.streamingAssetsPath, fileBaseName + ".json");
            string musicPath = Path.Combine(Application.streamingAssetsPath, fileBaseName + ".wav");

            // ���� JSON
            string json;
#if UNITY_ANDROID && !UNITY_EDITOR
            UnityWebRequest jsonRequest = UnityWebRequest.Get(jsonPath);
            yield return jsonRequest.SendWebRequest();
            json = jsonRequest.downloadHandler.text;
#else
            if (!File.Exists(jsonPath))
            {
                Debug.LogError("�Ҳ��� JSON �ļ���" + jsonPath);
                yield break;
            }
            json = File.ReadAllText(jsonPath);
#endif

            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError("JSON ����ʧ�ܣ�");
                yield break;
            }

            NotesContainer.Instance.json = json;
            Debug.Log("JSON ���سɹ�");

            var editData = JsonUtility.FromJson<MusicDTO.EditData>(json);
            if (editData == null)
            {
                Debug.LogError("JSON ����ʧ�ܣ�");
                yield break;
            }

            // ������Ƶ
            UnityWebRequest musicRequest = UnityWebRequestMultimedia.GetAudioClip("file://" + musicPath, AudioType.WAV);
            yield return musicRequest.SendWebRequest();

            if (musicRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("��Ƶ����ʧ�ܣ�" + musicRequest.error);
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(musicRequest);
            if (clip == null)
            {
                Debug.LogError("��Ƶ��Ч��");
                yield break;
            }

            NotesContainer.Instance.music = clip;
            NotesContainer.Instance.autoplay = false;

            Debug.Log($"��Ƶ������ɣ�{clip.length:F2}s, {clip.frequency}Hz");

            isLoaded = true;

            // ������ת��ť
            if (enterButton != null)
            {
                enterButton.gameObject.SetActive(true);
                Debug.Log("��Դ������ϣ���ť������");
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
                Debug.LogWarning("��Դ��δ�����꣬�޷�������Ϸ��");
            }
        }

        public bool IsLoaded()
        {
            return isLoaded;
        }
    }
}
