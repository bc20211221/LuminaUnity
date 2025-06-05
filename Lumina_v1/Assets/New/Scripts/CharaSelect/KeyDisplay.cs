using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Menu;

public class KeyDisplayUI : MonoBehaviour
{
    [Header("����")]
    public List<Text> keyDisplayTexts; // ������ʾ��λ��Text����б�
    public GameObject keyDisplayPanel; // �������м�λ��ʾ�����

    void Start()
    {
        UpdateKeyDisplay();
    }

    // �������й���ļ�λ��ʾ
    public void UpdateKeyDisplay()
    {
        int maxLanes = Mathf.Min(keyDisplayTexts.Count, PlayerSettings.Instance.keys.Length);

        for (int i = 0; i < maxLanes; i++)
        {
            KeyCode key = PlayerSettings.Instance.GetKeyCode(i);
            keyDisplayTexts[i].text = $"{key.ToString()}";
        }
    }

    // ��ʾ�����ؼ�λ���
    public void ToggleKeyDisplay(bool show)
    {
        keyDisplayPanel.SetActive(show);
    }
}