// SlotManager.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SlotManager : MonoBehaviour
{
    public static SlotManager Instance { get; private set; }

    public List<GameObject> slotUIs = new List<GameObject>();
    private List<Slot> slots = new List<Slot>();

    public System.Action<CharacterData> onCharacterRemoved;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeSlots();
            Debug.Log("SlotManager初始化完成");
        }
        else
        {
            Debug.LogWarning("重复创建SlotManager实例，销毁当前实例");
            Destroy(gameObject);
        }
    }

    void InitializeSlots()
    {
        slots.Clear();

        for (int i = 0; i < slotUIs.Count; i++)
        {
            GameObject slotUI = slotUIs[i];
            if (slotUI == null)
            {
                Debug.LogError($"槽位UI列表中索引 {i} 为空！");
                continue;
            }

            Image background = slotUI.GetComponent<Image>();
            if (background == null)
            {
                Debug.LogError($"槽位UI {slotUI.name} 缺少Image组件！");
                continue;
            }

            // 创建Slot组件
            Slot slot = slotUI.AddComponent<Slot>();
            slot.Initialize(background, i);
            slots.Add(slot);

            Debug.Log($"初始化槽位 {i}: {slotUI.name}");
        }
    }

    public int GetNextAvailableSlotIndex()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].GetCurrentCharacter() == null)
            {
                Debug.Log($"找到可用槽位: {i}");
                return i;
            }
        }

        Debug.Log("没有可用槽位");
        return -1;
    }

    public bool AddCharacterToSlot(CharacterData character, int slotIndex)
    {
        Debug.Log($"尝试将角色 {character.characterName} 添加到槽位 {slotIndex}");

        if (slotIndex < 0 || slotIndex >= slots.Count)
        {
            Debug.LogError($"无效的槽位索引: {slotIndex}");
            return false;
        }

        // 检查角色是否已经在某个槽位中
        foreach (var slot in slots)
        {
            if (slot.GetCurrentCharacter() == character)
            {
                Debug.LogError($"角色 {character.characterName} 已经在某个槽位中！");
                return false; // 角色已存在，返回false
            }
        }

        Slot targetSlot = slots[slotIndex];

        // 检查槽位是否已经被占用
        if (targetSlot.GetCurrentCharacter() != null)
        {
            Debug.LogError($"槽位 {slotIndex} 已经被占用！");
            return false;
        }

        targetSlot.SetCharacter(character);
        Debug.Log($"成功将角色 {character.characterName} 添加到槽位 {slotIndex}");
        return true;
    }

    public void RemoveCharacterFromSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count)
        {
            Debug.LogError($"无效的槽位索引: {slotIndex}");
            return;
        }

        Slot targetSlot = slots[slotIndex];
        CharacterData removedCharacter = targetSlot.GetCurrentCharacter();

        if (removedCharacter != null)
        {
            targetSlot.Clear();
            Debug.Log($"从槽位 {slotIndex} 移除角色: {removedCharacter.characterName}");

            // 通知角色库管理器解锁角色卡片
            CharacterLibraryManager.Instance.UnlockCharacterCard(removedCharacter);

            // 通知角色移除事件
            if (onCharacterRemoved != null)
            {
                onCharacterRemoved(removedCharacter);
            }
        }
        else
        {
            Debug.Log($"槽位 {slotIndex} 为空");
        }
    }

    public bool AreAllSlotsFull()
    {
        foreach (var slot in slots)
        {
            if (slot.GetCurrentCharacter() == null)
            {
                return false;
            }
        }
        return true;
    }

    // 提供公共方法获取slots列表
    public List<Slot> GetSlots()
    {
        return slots;
    }
}