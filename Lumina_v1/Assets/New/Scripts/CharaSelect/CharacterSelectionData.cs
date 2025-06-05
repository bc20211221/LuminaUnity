using UnityEngine;
using System.Collections.Generic;

public class CharacterSelectionData : MonoBehaviour
{
    public static CharacterSelectionData Instance { get; private set; }
    public List<CharacterData> selectedCharacters = new List<CharacterData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public CharacterData GetCharacterDataById(int id)
    {
        foreach (CharacterData data in selectedCharacters)
        {
            if (data.id == id)
            {
                return data;
            }
        }
        return null;
    }

    // 新增方法：获取所有已选择角色的 ID 列表
    public int[] GetSelectedCharacterIds()
    {
        List<int> ids = new List<int>();
        foreach (CharacterData data in selectedCharacters)
        {
            ids.Add(data.id);
        }
        return ids.ToArray();
    }
}