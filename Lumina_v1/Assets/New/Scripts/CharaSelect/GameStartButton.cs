using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameStartButton : MonoBehaviour
{
    public Button startGameButton;
    public string nextSceneName;
    public GameObject noticeLayer; // Notice图层
    public float noticeDuration = 3f; // 显示时长(秒)
    private bool isNoticeShowing = false; // 是否正在显示Notice
    private float noticeTimer = 0f; // 计时器

    void Start()
    {
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(OnStartGameClicked);
        }

        // 初始隐藏Notice图层
        if (noticeLayer != null)
            noticeLayer.SetActive(false);
    }

    void Update()
    {
        // 如果Notice正在显示，更新计时器
        if (isNoticeShowing)
        {
            noticeTimer += Time.deltaTime;
            if (noticeTimer >= noticeDuration)
            {
                // 3秒后隐藏Notice
                HideNotice();
            }
        }
    }

    void OnStartGameClicked()
    {
        if (SlotManager.Instance.AreAllSlotsFull())
        {
            // 槽位已满，跳转到下一个场景
            List<CharacterData> selectedCharacters = new List<CharacterData>();
            List<Slot> slots = SlotManager.Instance.GetSlots();
            foreach (Slot slot in slots)
            {
                CharacterData character = slot.GetCurrentCharacter();
                if (character != null)
                {
                    selectedCharacters.Add(character);
                }
            }

            // 保存角色信息到单例中，以便在下一个场景中使用
            CharacterSelectionData.Instance.selectedCharacters = selectedCharacters;


            // 加载下一个场景
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            // 槽位未满，显示Notice
            if (isNoticeShowing)
            {
                HideNotice();
            }
            
            ShowNotice();
        }
    }

    void ShowNotice()
    {
        if (noticeLayer != null)
        {
            noticeLayer.SetActive(true);
            isNoticeShowing = true;
            noticeTimer = 0f;
        }
    }

    void HideNotice()
    {
        if (noticeLayer != null)
        {
            noticeLayer.SetActive(false);
            isNoticeShowing = false;
        }
    }
}