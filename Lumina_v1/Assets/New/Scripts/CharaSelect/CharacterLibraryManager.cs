using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharacterLibraryManager : MonoBehaviour
{
    public static CharacterLibraryManager Instance { get; private set; }

    [Header("设置")]
    public RectTransform contentParent;
    public GameObject characterCardPrefab;
    public List<CharacterData> allCharacters = new List<CharacterData>();

    [Header("滚动设置")]
    public float snapSpeed = 5f;
    public int cardsPerPage = 3;

    private ScrollRect scrollRect;
    private float[] pagePositions;
    private int currentPage = 0;
    private bool isSnapping = false;

    // 角色卡片列表
    private List<CharacterCard> characterCards = new List<CharacterCard>();

    // 角色选择事件
    public System.Action<CharacterData> onCharacterSelected;

    void Awake()
    {
        // 单例初始化
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("CharacterLibraryManager单例初始化成功");
        }
        else
        {
            Debug.LogWarning("CharacterLibraryManager单例已存在，销毁当前实例");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        scrollRect = GetComponentInParent<ScrollRect>();

        // 检查必要组件
        if (contentParent == null)
        {
            Debug.LogError("ContentParent不能为空，请在Inspector中设置");
            return;
        }

        if (characterCardPrefab == null)
        {
            Debug.LogError("CharacterCardPrefab不能为空，请在Inspector中设置");
            return;
        }

        GenerateCharacterCards();
        CalculatePagePositions();

        // 注册槽位管理器的角色移除事件
        if (SlotManager.Instance != null)
        {
            SlotManager.Instance.onCharacterRemoved += OnCharacterRemoved;
            Debug.Log("已注册SlotManager的角色移除事件");
        }
        else
        {
            Debug.LogError("无法注册角色移除事件，SlotManager实例为空");
        }
    }

    // 生成角色卡片
    void GenerateCharacterCards()
    {
        // 清除所有卡片
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        characterCards.Clear();

        // 创建新卡片
        foreach (var character in allCharacters)
        {
            if (character == null)
            {
                Debug.LogError("角色数据为空，跳过创建卡片");
                continue;
            }

            GameObject card = Instantiate(characterCardPrefab, contentParent);
            Debug.Log($"实例化角色卡片: {character.characterName}");

            // 确保卡片在游戏中激活
            if (!card.activeSelf)
            {
                Debug.LogWarning($"角色卡片 {card.name} 未激活，自动激活");
                card.SetActive(true);
            }

            // 获取卡片的CharacterCard组件
            CharacterCard cardComponent = card.GetComponent<CharacterCard>();

            if (cardComponent == null)
            {
                Debug.LogError($"预制体中未找到CharacterCard组件，角色名称: {character.characterName}");
                continue;
            }

            // 确保组件已启用
            if (!cardComponent.enabled)
            {
                Debug.LogWarning($"CharacterCard组件在 {card.name} 中未启用，自动启用");
                cardComponent.enabled = true;
            }

            // 获取卡片的Button组件
            Button cardButton = card.GetComponent<Button>();

            if (cardButton == null)
            {
                Debug.LogError($"预制体中未找到Button组件，角色名称: {character.characterName}");
                continue;
            }

            // 确保Button已启用
            if (!cardButton.enabled)
            {
                Debug.LogWarning($"Button组件在 {card.name} 中未启用，自动启用");
                cardButton.enabled = true;
            }

            // 初始化卡片
            cardComponent.Initialize(character);
            cardComponent.OnCharacterClicked += OnCharacterCardClicked;
            characterCards.Add(cardComponent);

            Debug.Log($"成功初始化角色卡片: {character.characterName}");
        }

        Debug.Log($"生成完成，共 {characterCards.Count} 张角色卡片");
    }

    // 卡片点击事件
    void OnCharacterCardClicked(CharacterData character)
    {
        Debug.Log($"CharacterLibraryManager收到点击事件，角色: {character.characterName}");

        // 获取下一个可用槽位索引
        int nextSlotIndex = SlotManager.Instance.GetNextAvailableSlotIndex();
        Debug.Log($"下一个可用槽位索引: {nextSlotIndex}");

        // 如果有可用槽位，添加角色
        if (nextSlotIndex >= 0)
        {
            // 通知槽位管理器添加角色到指定槽位
            if (SlotManager.Instance.AddCharacterToSlot(character, nextSlotIndex))
            {
                Debug.Log($"成功将角色 {character.characterName} 添加到槽位 {nextSlotIndex}");

                // 更新角色卡片状态
                UpdateCharacterSelection(character, true);

                // 触发角色选择事件
                if (onCharacterSelected != null)
                {
                    onCharacterSelected(character);
                    Debug.Log($"触发角色选择事件: {character.characterName}");
                }
            }
            else
            {
                Debug.LogWarning($"无法将角色 {character.characterName} 添加到槽位 {nextSlotIndex}");
            }
        }
        else
        {
            Debug.Log("没有可用槽位");
            // 处理没有可用槽位的情况，可添加提示逻辑
        }
    }

    // 角色从槽位移除事件
    void OnCharacterRemoved(CharacterData character)
    {
        Debug.Log($"收到角色移除事件: {character.characterName}");
        UpdateCharacterSelection(character, false);
        UnlockCharacterCard(character);
    }

    // 更新角色选择状态
    public void UpdateCharacterSelection(CharacterData character, bool isSelected)
    {
        foreach (var card in characterCards)
        {
            if (card.GetCharacter() == character)
            {
                Debug.Log($"更新角色状态: {character.characterName}, 选择状态: {isSelected}");
                card.SetSelected(isSelected);
                break;
            }
        }
    }

    // 解锁角色卡片
    public void UnlockCharacterCard(CharacterData character)
    {
        foreach (var card in characterCards)
        {
            if (card.GetCharacter() == character)
            {
                card.Unlock();
                break;
            }
        }
    }

    // 计算页面位置
    void CalculatePagePositions()
    {
        int totalPages = Mathf.Max(1, Mathf.CeilToInt((allCharacters.Count - cardsPerPage) / (float)cardsPerPage) + 1);
        pagePositions = new float[totalPages];

        for (int i = 0; i < totalPages; i++)
        {
            pagePositions[i] = i / (float)(totalPages - 1);
            if (totalPages == 1) pagePositions[i] = 0f;  // 如果只有一页，设置为0
        }

        Debug.Log($"计算页面位置完成: 共 {totalPages} 页");
    }

    // 页面滚动
    void Update()
    {
        if (isSnapping)
        {
            float targetPosition = pagePositions[currentPage];
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(
                scrollRect.horizontalNormalizedPosition,
                targetPosition,
                snapSpeed * Time.deltaTime
            );

            // 检查是否到达目标位置
            if (Mathf.Abs(scrollRect.horizontalNormalizedPosition - targetPosition) < 0.001f)
            {
                isSnapping = false;
                Debug.Log($"已滚动到页面 {currentPage}");
            }
        }
    }

    // 切换上一页
/*    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            isSnapping = true;
            Debug.Log($"切换到上一页: {currentPage}");
        }
        else
        {
            Debug.Log("已经是第一页");
        }
    }

    // 切换下一页
    public void NextPage()
    {
        if (currentPage < pagePositions.Length - 1)
        {
            currentPage++;
            isSnapping = true;
            Debug.Log($"切换到下一页: {currentPage}");
        }
        else
        {
            Debug.Log("已经是最后一页");
        }
    }*/

    // 滚动结束时调用（可自动调用）
/*    public void OnScrollEnd()
    {
        if (!isSnapping)
        {
            float minDistance = float.MaxValue;
            int closestPage = currentPage;

            // 找到最近的页面
            for (int i = 0; i < pagePositions.Length; i++)
            {
                float distance = Mathf.Abs(scrollRect.horizontalNormalizedPosition - pagePositions[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPage = i;
                }
            }

            // 如果需要切换页面
            if (closestPage != currentPage)
            {
                currentPage = closestPage;
                isSnapping = true;
                Debug.Log($"自动滚动到页面: {currentPage}");
            }
        }
    }*/
}