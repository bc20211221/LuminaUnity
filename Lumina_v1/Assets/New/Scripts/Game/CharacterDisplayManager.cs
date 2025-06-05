using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharacterDisplayManager : MonoBehaviour
{
    [Header("UI References")]
    public Image[] characterImages; // ��Inspector�з���4��UI Image���
    public Text[] characterNames;   // ��ѡ����ʾ��ɫ���Ƶ�Text���
    public GameObject noSelectionNotice; // ��û��ѡ���ɫʱ��ʾ����ʾ

    private void Start()
    {
        DisplaySelectedCharacters();
    }

    private void DisplaySelectedCharacters()
    {
        // ��ȡѡ�еĽ�ɫ����
        List<CharacterData> selectedCharacters = CharacterSelectionData.Instance.selectedCharacters;

        // ��֤����
        if (selectedCharacters == null || selectedCharacters.Count == 0)
        {
            Debug.LogWarning("û��ѡ���κν�ɫ");
            ShowNoSelectionNotice();
            return;
        }

        // ��ʾ��ɫͼ��
        for (int i = 0; i < Mathf.Min(selectedCharacters.Count, characterImages.Length); i++)
        {
            CharacterData character = selectedCharacters[i];
            Image targetImage = characterImages[i];

            if (character == null || targetImage == null)
            {
                Debug.LogWarning($"���� {i} ���Ľ�ɫ��ͼ������Ϊ��");
                continue;
            }

            // ���ý�ɫͼ��
            if (character.characterIcon != null)
            {
                targetImage.sprite = character.characterIcon;
                targetImage.gameObject.SetActive(true);
                Debug.Log($"�ɹ���ʾ��ɫ {character.characterName} ��ͼ��");

                // ��ѡ�����ý�ɫ����
                if (characterNames != null && i < characterNames.Length && characterNames[i] != null)
                {
                    characterNames[i].text = character.characterName;
                }
            }
            else
            {
                Debug.LogError($"��ɫ {character.characterName} ��ͼ��Ϊ��");
            }
        }
    }

    private void ShowNoSelectionNotice()
    {
        if (noSelectionNotice != null)
        {
            noSelectionNotice.SetActive(true);
        }
    }
}