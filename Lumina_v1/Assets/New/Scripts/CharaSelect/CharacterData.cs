// CharacterData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Character Data")]
public class CharacterData : ScriptableObject
{
    public int id; // ���� ID �ֶ�
    public string characterName;
    public Sprite characterIcon;
    public GameObject characterPrefab;
    public Sprite backgroundImage;
    // ������ɫ����
    public string characterDescription;

}