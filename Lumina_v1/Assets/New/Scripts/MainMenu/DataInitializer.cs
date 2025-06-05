using UnityEngine;
using System.IO;

public class DataInitializer : MonoBehaviour
{
    // 标记是否已初始化（避免在场景切换时重复初始化）
    private static bool initialized = false;

    [Header("数据清除设置")]
    [Tooltip("是否清除PlayerPrefs数据")]
    public bool clearPlayerPrefs = true;

    [Tooltip("是否删除保存的文件")]
    public bool deleteSavedFiles = true;

    [Tooltip("要删除的文件路径列表")]
    public string[] filePathsToDelete;

    void Awake()
    {
        // 确保只初始化一次
        if (!initialized)
        {
            InitializeGameData();
            initialized = true;
        }

        // 保持该对象在场景切换时不被销毁
        DontDestroyOnLoad(gameObject);
    }

    void InitializeGameData()
    {
        Debug.Log("初始化游戏数据...");

        // 清除PlayerPrefs数据
        if (clearPlayerPrefs)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("已清除PlayerPrefs数据");
        }

        // 删除指定的保存文件
        if (deleteSavedFiles && filePathsToDelete != null)
        {
            foreach (string filePath in filePathsToDelete)
            {
                DeleteFileIfExists(filePath);
            }
        }

        // 可以添加其他数据清除逻辑...

        Debug.Log("游戏数据初始化完成");
    }

    void DeleteFileIfExists(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return;

        try
        {
            // 获取完整路径（支持相对路径和绝对路径）
            string fullPath = Path.IsPathRooted(filePath)
                ? filePath
                : Path.Combine(Application.persistentDataPath, filePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                Debug.Log($"已删除文件: {fullPath}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"删除文件时出错: {e.Message}");
        }
    }
}