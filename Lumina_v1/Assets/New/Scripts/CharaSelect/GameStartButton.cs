using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameStartButton : MonoBehaviour
{
    public Button startGameButton;
    public string nextSceneName;
    public GameObject noticeLayer; // Noticeͼ��
    public float noticeDuration = 3f; // ��ʾʱ��(��)
    private bool isNoticeShowing = false; // �Ƿ�������ʾNotice
    private float noticeTimer = 0f; // ��ʱ��

    void Start()
    {
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(OnStartGameClicked);
        }

        // ��ʼ����Noticeͼ��
        if (noticeLayer != null)
            noticeLayer.SetActive(false);
    }

    void Update()
    {
        // ���Notice������ʾ�����¼�ʱ��
        if (isNoticeShowing)
        {
            noticeTimer += Time.deltaTime;
            if (noticeTimer >= noticeDuration)
            {
                // 3�������Notice
                HideNotice();
            }
        }
    }

    void OnStartGameClicked()
    {
        if (SlotManager.Instance.AreAllSlotsFull())
        {
            // ��λ��������ת����һ������
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

            // �����ɫ��Ϣ�������У��Ա�����һ��������ʹ��
            CharacterSelectionData.Instance.selectedCharacters = selectedCharacters;


            // ������һ������
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            // ��λδ������ʾNotice
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