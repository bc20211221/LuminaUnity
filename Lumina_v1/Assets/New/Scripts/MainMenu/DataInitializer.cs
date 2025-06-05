using UnityEngine;
using System.IO;

public class DataInitializer : MonoBehaviour
{
    // ����Ƿ��ѳ�ʼ���������ڳ����л�ʱ�ظ���ʼ����
    private static bool initialized = false;

    [Header("�����������")]
    [Tooltip("�Ƿ����PlayerPrefs����")]
    public bool clearPlayerPrefs = true;

    [Tooltip("�Ƿ�ɾ��������ļ�")]
    public bool deleteSavedFiles = true;

    [Tooltip("Ҫɾ�����ļ�·���б�")]
    public string[] filePathsToDelete;

    void Awake()
    {
        // ȷ��ֻ��ʼ��һ��
        if (!initialized)
        {
            InitializeGameData();
            initialized = true;
        }

        // ���ָö����ڳ����л�ʱ��������
        DontDestroyOnLoad(gameObject);
    }

    void InitializeGameData()
    {
        Debug.Log("��ʼ����Ϸ����...");

        // ���PlayerPrefs����
        if (clearPlayerPrefs)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("�����PlayerPrefs����");
        }

        // ɾ��ָ���ı����ļ�
        if (deleteSavedFiles && filePathsToDelete != null)
        {
            foreach (string filePath in filePathsToDelete)
            {
                DeleteFileIfExists(filePath);
            }
        }

        // �������������������߼�...

        Debug.Log("��Ϸ���ݳ�ʼ�����");
    }

    void DeleteFileIfExists(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return;

        try
        {
            // ��ȡ����·����֧�����·���;���·����
            string fullPath = Path.IsPathRooted(filePath)
                ? filePath
                : Path.Combine(Application.persistentDataPath, filePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                Debug.Log($"��ɾ���ļ�: {fullPath}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"ɾ���ļ�ʱ����: {e.Message}");
        }
    }
}