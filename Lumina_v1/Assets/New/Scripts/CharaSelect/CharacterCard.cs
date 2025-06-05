using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems; 

public class CharacterCard : MonoBehaviour
{
    [Header("UI���")]
    public Image iconImage;
    public Text nameText;
    public Image selectionOverlay;
    public Button cardButton;
    public GameObject backgroundGameObject;
    public Text characterDescriptionText;
    // ������CharaInfo���������������
    public GameObject charaInfoObject;
    public Text charaNameText;
    public Text charaInfoText;


    [Header("״̬����")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;
    public Color disabledColor = Color.gray;

    private CharacterData character;
    private bool isSelected = false;
    private bool isLocked = false;


    public System.Action<CharacterData> OnCharacterClicked;

    void Start()
    {
        Debug.Log($"CharacterCard Start() - {name}");
        if (cardButton == null)
        {
            cardButton = gameObject.GetComponent<Button>();
            if (cardButton == null)
            {
                Debug.LogError("Button��������ڣ�");
                return;
            }
        }
        cardButton.onClick.RemoveAllListeners();
        cardButton.onClick.AddListener(OnClick);

        // ���EventTrigger�����ע���¼�
        EventTrigger trigger = cardButton.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((eventData) => { OnMouseEnter((PointerEventData)eventData); });
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((eventData) => { OnMouseExit((PointerEventData)eventData); });
        trigger.triggers.Add(entryExit);

        // ��ȡ������ButtonController�ű�
        ButtonController buttonController = GetComponent<ButtonController>();
        if (buttonController != null)
        {
            buttonController.enabled = true;
        }
    }

    // ��ʼ����ɫ��Ƭ
    public void Initialize(CharacterData character)
    {
        this.character = character;
        Debug.Log($"��ʼ����ɫ��Ƭ: {character.characterName}");

        if (iconImage != null && character.characterIcon != null)
        {
            iconImage.sprite = character.characterIcon;
            iconImage.enabled = true;
        }

        if (nameText != null)
        {
            nameText.text = character.characterName;
            nameText.enabled = true;
        }
        if(backgroundGameObject != null)
        {
            Image backgroundImage = backgroundGameObject.GetComponent<Image>();
            if (backgroundImage != null)
            {
                backgroundImage.enabled = true;
            }
        }
        UpdateVisualState();
    }

    void OnClick()
    {
        Debug.Log($"��ɫ��Ƭ���: {character.characterName}");
        // ���Ԥ���������Ƿ�Ϊ����δ������
        if (character.characterName == "��δ����")
        {
            return;
        }

        if (isLocked)
        {
            RemoveFromSlot();
            return;
        }

        int nextSlotIndex = SlotManager.Instance.GetNextAvailableSlotIndex();
        Debug.Log($"��һ�����ò�λ: {nextSlotIndex}");

        if (nextSlotIndex >= 0)
        {
            if (SlotManager.Instance.AddCharacterToSlot(character, nextSlotIndex))
            {
                isLocked = true;
                UpdateVisualState();

                if (OnCharacterClicked != null)
                {
                    OnCharacterClicked(character);
                }
            }
        }
        else
        {
            Debug.Log("û�п��ò�λ");
        }
    }

    void RemoveFromSlot()
    {
        List<Slot> slots = SlotManager.Instance.GetSlots();
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].GetCurrentCharacter() == character)
            {
                SlotManager.Instance.RemoveCharacterFromSlot(i);
                isLocked = false;
                UpdateVisualState();
                break;
            }
        }
    }

    void UpdateVisualState()
    {
        if (cardButton != null)
        {
            if (isLocked)
            {
                cardButton.image.color = disabledColor;
            }
            else if (isSelected)
            {
                cardButton.image.color = selectedColor;
            }
            else
            {
                cardButton.image.color = normalColor;
            }
        }
    }

    public CharacterData GetCharacter()
    {
        return character;
    }

    public bool IsLocked()
    {
        return isLocked;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisualState();
    }

    // ������ɫ��Ƭ
    public void Unlock()
    {
        isLocked = false;
        UpdateVisualState();
    }
    // ��������¼�����

    void OnMouseEnter(PointerEventData eventData)
    {
        if (charaInfoObject != null && character != null)
        {
            charaInfoObject.SetActive(true);
            charaNameText.text = character.characterName;
            charaInfoText.text = character.characterDescription;

            // ����CharaInfoObject��λ��
            RectTransform cardRect = cardButton.GetComponent<RectTransform>();
            RectTransform infoRect = charaInfoObject.GetComponent<RectTransform>();
            infoRect.anchoredPosition = new Vector2(cardRect.anchoredPosition.x-645, cardRect.anchoredPosition.y - cardRect.rect.height);
        }
    }

    void OnMouseExit(PointerEventData eventData)
    {
        if (charaInfoObject != null)
        {
            charaInfoObject.SetActive(false);
        }
    }
}