using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharacterDisplayManager : MonoBehaviour
{
    [Header("UI References")]
    public Image[] characterImages; // 在Inspector中分配4个UI Image组件
    public Text[] characterNames;   // 可选：显示角色名称的Text组件
    public GameObject noSelectionNotice; // 当没有选择角色时显示的提示

    private void Start()
    {
        DisplaySelectedCharacters();
    }

    private void DisplaySelectedCharacters()
    {
        // 获取选中的角色数据
        List<CharacterData> selectedCharacters = CharacterSelectionData.Instance.selectedCharacters;

        // 验证数据
        if (selectedCharacters == null || selectedCharacters.Count == 0)
        {
            Debug.LogWarning("没有选中任何角色");
            ShowNoSelectionNotice();
            return;
        }

        // 显示角色图像
        for (int i = 0; i < Mathf.Min(selectedCharacters.Count, characterImages.Length); i++)
        {
            CharacterData character = selectedCharacters[i];
            Image targetImage = characterImages[i];

            if (character == null || targetImage == null)
            {
                Debug.LogWarning($"索引 {i} 处的角色或图像引用为空");
                continue;
            }

            // 设置角色图像
            if (character.characterIcon != null)
            {
                targetImage.sprite = character.characterIcon;
                targetImage.gameObject.SetActive(true);
                Debug.Log($"成功显示角色 {character.characterName} 的图像");

                // 可选：设置角色名称
                if (characterNames != null && i < characterNames.Length && characterNames[i] != null)
                {
                    characterNames[i].text = character.characterName;
                }
            }
            else
            {
                Debug.LogError($"角色 {character.characterName} 的图标为空");
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