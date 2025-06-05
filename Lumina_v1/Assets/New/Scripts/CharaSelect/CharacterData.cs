// CharacterData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Character Data")]
public class CharacterData : ScriptableObject
{
    public int id; // 新增 ID 字段
    public string characterName;
    public Sprite characterIcon;
    public GameObject characterPrefab;
    public Sprite backgroundImage;
    // 其他角色属性
    public string characterDescription;

}