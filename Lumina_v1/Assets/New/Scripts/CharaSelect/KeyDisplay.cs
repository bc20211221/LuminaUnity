using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Menu;

public class KeyDisplayUI : MonoBehaviour
{
    [Header("配置")]
    public List<Text> keyDisplayTexts; // 用于显示键位的Text组件列表
    public GameObject keyDisplayPanel; // 包含所有键位显示的面板

    void Start()
    {
        UpdateKeyDisplay();
    }

    // 更新所有轨道的键位显示
    public void UpdateKeyDisplay()
    {
        int maxLanes = Mathf.Min(keyDisplayTexts.Count, PlayerSettings.Instance.keys.Length);

        for (int i = 0; i < maxLanes; i++)
        {
            KeyCode key = PlayerSettings.Instance.GetKeyCode(i);
            keyDisplayTexts[i].text = $"{key.ToString()}";
        }
    }

    // 显示或隐藏键位面板
    public void ToggleKeyDisplay(bool show)
    {
        keyDisplayPanel.SetActive(show);
    }
}