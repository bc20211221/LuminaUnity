using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [Header("UI组件")]
    public Image backgroundImage;
    public Image iconImage;
    public Text nameText;
    public Button slotButton;

    [Header("颜色设置")]
    public Color emptySlotColor = Color.gray;
    public Color filledSlotColor = Color.white;

    private CharacterData currentCharacter;
    private int slotIndex;

    // 初始化槽位
    public void Initialize(Image background, int index)
    {
        slotIndex = index;
        backgroundImage = background;

        // 添加空值检查
        if (backgroundImage != null)
        {
            backgroundImage.color = emptySlotColor;

            // 创建图标Image组件
            CreateIconImage();

            // 创建名称Text组件
            //CreateNameText();

            // 设置槽位点击事件
            if (slotButton == null)
            {
                slotButton = gameObject.AddComponent<Button>();
            }

            // 添加空值检查
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

    // 创建图标Image组件
    private void CreateIconImage()
    {
        if (iconImage == null)
        {
            GameObject iconObj = new GameObject("Icon");

            // 添加空值检查
            if (backgroundImage.transform != null)
            {
                iconObj.transform.SetParent(backgroundImage.transform, false);

                // 设置图标位置和大小
                RectTransform rect = iconObj.AddComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = new Vector2(10, 10);  // 内边距
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

    // 创建名称Text组件
/*    private void CreateNameText()
    {
        if (nameText == null)
        {
            GameObject nameObj = new GameObject("Name");

            // 添加空值检查
            if (backgroundImage.transform != null)
            {
                nameObj.transform.SetParent(backgroundImage.transform, false);

                // 设置文本位置和大小
                RectTransform rect = nameObj.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.sizeDelta = new Vector2(0, 20);
                rect.anchoredPosition = new Vector2(0, 5);

                nameText = nameObj.AddComponent<Text>();

                // 使用LegacyRuntime.ttf字体
                try
                {
                    nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to load font: {e.Message}");
                    // 备选方案：使用默认字体
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

    // 槽位点击事件
    void OnSlotClicked()
    {
        // 如果槽位中有角色，则移除该角色
        if (currentCharacter != null)
        {
            // 通知槽位管理器移除角色
            SlotManager.Instance.RemoveCharacterFromSlot(slotIndex);
        }
    }

    // 设置槽位中的角色
    public void SetCharacter(CharacterData character)
    {
        currentCharacter = character;

        if (character != null)
        {
            // 显示角色信息
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
            // 清空槽位
            Clear();
        }
    }

    // 获取当前槽位中的角色
    public CharacterData GetCurrentCharacter()
    {
        return currentCharacter;
    }

    // 清空槽位
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