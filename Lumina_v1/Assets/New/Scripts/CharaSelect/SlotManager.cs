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
            Debug.Log("SlotManager��ʼ�����");
        }
        else
        {
            Debug.LogWarning("�ظ�����SlotManagerʵ�������ٵ�ǰʵ��");
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
                Debug.LogError($"��λUI�б������� {i} Ϊ�գ�");
                continue;
            }

            Image background = slotUI.GetComponent<Image>();
            if (background == null)
            {
                Debug.LogError($"��λUI {slotUI.name} ȱ��Image�����");
                continue;
            }

            // ����Slot���
            Slot slot = slotUI.AddComponent<Slot>();
            slot.Initialize(background, i);
            slots.Add(slot);

            Debug.Log($"��ʼ����λ {i}: {slotUI.name}");
        }
    }

    public int GetNextAvailableSlotIndex()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].GetCurrentCharacter() == null)
            {
                Debug.Log($"�ҵ����ò�λ: {i}");
                return i;
            }
        }

        Debug.Log("û�п��ò�λ");
        return -1;
    }

    public bool AddCharacterToSlot(CharacterData character, int slotIndex)
    {
        Debug.Log($"���Խ���ɫ {character.characterName} ��ӵ���λ {slotIndex}");

        if (slotIndex < 0 || slotIndex >= slots.Count)
        {
            Debug.LogError($"��Ч�Ĳ�λ����: {slotIndex}");
            return false;
        }

        // ����ɫ�Ƿ��Ѿ���ĳ����λ��
        foreach (var slot in slots)
        {
            if (slot.GetCurrentCharacter() == character)
            {
                Debug.LogError($"��ɫ {character.characterName} �Ѿ���ĳ����λ�У�");
                return false; // ��ɫ�Ѵ��ڣ�����false
            }
        }

        Slot targetSlot = slots[slotIndex];

        // ����λ�Ƿ��Ѿ���ռ��
        if (targetSlot.GetCurrentCharacter() != null)
        {
            Debug.LogError($"��λ {slotIndex} �Ѿ���ռ�ã�");
            return false;
        }

        targetSlot.SetCharacter(character);
        Debug.Log($"�ɹ�����ɫ {character.characterName} ��ӵ���λ {slotIndex}");
        return true;
    }

    public void RemoveCharacterFromSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count)
        {
            Debug.LogError($"��Ч�Ĳ�λ����: {slotIndex}");
            return;
        }

        Slot targetSlot = slots[slotIndex];
        CharacterData removedCharacter = targetSlot.GetCurrentCharacter();

        if (removedCharacter != null)
        {
            targetSlot.Clear();
            Debug.Log($"�Ӳ�λ {slotIndex} �Ƴ���ɫ: {removedCharacter.characterName}");

            // ֪ͨ��ɫ�������������ɫ��Ƭ
            CharacterLibraryManager.Instance.UnlockCharacterCard(removedCharacter);

            // ֪ͨ��ɫ�Ƴ��¼�
            if (onCharacterRemoved != null)
            {
                onCharacterRemoved(removedCharacter);
            }
        }
        else
        {
            Debug.Log($"��λ {slotIndex} Ϊ��");
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

    // �ṩ����������ȡslots�б�
    public List<Slot> GetSlots()
    {
        return slots;
    }
}