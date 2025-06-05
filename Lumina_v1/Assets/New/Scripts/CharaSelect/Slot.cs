using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [Header("UI���")]
    public Image backgroundImage;
    public Image iconImage;
    public Text nameText;
    public Button slotButton;

    [Header("��ɫ����")]
    public Color emptySlotColor = Color.gray;
    public Color filledSlotColor = Color.white;

    private CharacterData currentCharacter;
    private int slotIndex;

    // ��ʼ����λ
    public void Initialize(Image background, int index)
    {
        slotIndex = index;
        backgroundImage = background;

        // ��ӿ�ֵ���
        if (backgroundImage != null)
        {
            backgroundImage.color = emptySlotColor;

            // ����ͼ��Image���
            CreateIconImage();

            // ��������Text���
            //CreateNameText();

            // ���ò�λ����¼�
            if (slotButton == null)
            {
                slotButton = gameObject.AddComponent<Button>();
            }

            // ��ӿ�ֵ���
            if (slotButton != null)
            {
                slotButton.onClick.AddListener(OnSlotClicked);
            }
            else
            {
                Debug.LogError("Failed to add Button component in Initialize()");
            }
        }
        else
        {
            Debug.LogError("backgroundImage is null in Initialize()");
        }
    }

    // ����ͼ��Image���
    private void CreateIconImage()
    {
        if (iconImage == null)
        {
            GameObject iconObj = new GameObject("Icon");

            // ��ӿ�ֵ���
            if (backgroundImage.transform != null)
            {
                iconObj.transform.SetParent(backgroundImage.transform, false);

                // ����ͼ��λ�úʹ�С
                RectTransform rect = iconObj.AddComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = new Vector2(10, 10);  // �ڱ߾�
                rect.offsetMax = new Vector2(-10, -10);

                iconImage = iconObj.AddComponent<Image>();
                iconImage.enabled = false;
            }
            else
            {
                Debug.LogError("backgroundImage.transform is null in CreateIconImage()");
            }
        }
    }

    // ��������Text���
/*    private void CreateNameText()
    {
        if (nameText == null)
        {
            GameObject nameObj = new GameObject("Name");

            // ��ӿ�ֵ���
            if (backgroundImage.transform != null)
            {
                nameObj.transform.SetParent(backgroundImage.transform, false);

                // �����ı�λ�úʹ�С
                RectTransform rect = nameObj.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.sizeDelta = new Vector2(0, 20);
                rect.anchoredPosition = new Vector2(0, 5);

                nameText = nameObj.AddComponent<Text>();

                // ʹ��LegacyRuntime.ttf����
                try
                {
                    nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to load font: {e.Message}");
                    // ��ѡ������ʹ��Ĭ������
                    nameText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                }

                nameText.alignment = TextAnchor.MiddleCenter;
                nameText.text = "";
                nameText.color = Color.black;
            }
            else
            {
                Debug.LogError("backgroundImage.transform is null in CreateNameText()");
            }
        }
    }*/

    // ��λ����¼�
    void OnSlotClicked()
    {
        // �����λ���н�ɫ�����Ƴ��ý�ɫ
        if (currentCharacter != null)
        {
            // ֪ͨ��λ�������Ƴ���ɫ
            SlotManager.Instance.RemoveCharacterFromSlot(slotIndex);
        }
    }

    // ���ò�λ�еĽ�ɫ
    public void SetCharacter(CharacterData character)
    {
        currentCharacter = character;

        if (character != null)
        {
            // ��ʾ��ɫ��Ϣ
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

            if (backgroundImage != null)
            {
                backgroundImage.color = filledSlotColor;
            }
        }
        else
        {
            // ��ղ�λ
            Clear();
        }
    }

    // ��ȡ��ǰ��λ�еĽ�ɫ
    public CharacterData GetCurrentCharacter()
    {
        return currentCharacter;
    }

    // ��ղ�λ
    public void Clear()
    {
        currentCharacter = null;

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }

        if (nameText != null)
        {
            nameText.text = " ";
            nameText.enabled = true;
        }

        if (backgroundImage != null)
        {
            backgroundImage.color = emptySlotColor;
        }
    }
}