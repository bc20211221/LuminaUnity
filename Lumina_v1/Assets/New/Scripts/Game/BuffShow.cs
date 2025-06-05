using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

using Game.Process;
using TMPro;

public class BuffShow : MonoBehaviour
{
    [Header("Buff对应图片组件")]
    [Tooltip("按顺序对应不同Buff的图片")]
    [SerializeField] private List<Image> buffImages = new List<Image>();

    [Header("Buff对应文本组件")]
    [Tooltip("按顺序对应不同Buff图片下的文本组件（需与buffImages顺序一致）")]
    [SerializeField] private List<TextMeshProUGUI> buffTexts = new List<TextMeshProUGUI>(); // 修改为TextMeshProUGUI

    private RectTransform _containerRect;

    private Vector2[] buffPositions = new Vector2[]
    {
        new Vector2(-560, -102),
        new Vector2(-440, -102),
        new Vector2(-320, -102),
        new Vector2(-200,  -102),
        new Vector2(-80,  -102),
        new Vector2(40,  -102),
        new Vector2(160,  -102),
        new Vector2(280,  -102),
        new Vector2(400,  -102),
        new Vector2(520,  -102)
    };

    private readonly Dictionary<ComboPresenter.BuffType, int> _buffIndexMap = new()
    {
        { ComboPresenter.BuffType.PurityBoost, 0 },
        { ComboPresenter.BuffType.BloodBoost, 1 },
        { ComboPresenter.BuffType.KillEnemyPurityBoost, 2 },
        { ComboPresenter.BuffType.OneAttackBoost, 3 },
        { ComboPresenter.BuffType.TwoAttackBoost, 4 },
        { ComboPresenter.BuffType.ThreeAttackBoost, 5 },
        { ComboPresenter.BuffType.FourAttackBoost, 6 },
        { ComboPresenter.BuffType.FiveAttackBoost, 7 },
        { ComboPresenter.BuffType.SixAttackBoost, 8 },
        { ComboPresenter.BuffType.EnergyEffBoost, 9 },
        { ComboPresenter.BuffType.BiologyDeathBoost, 10 }
    };

    // 记录上一次触发列表的长度（用于检测变化）
    private int _lastBuffListCount = 0;

    // 存储当前各Buff类型的出现次数（键：Buff类型，值：次数）
    private Dictionary<ComboPresenter.BuffType, int> _currentBuffCounts = new();

    private void Awake()
    {
        _containerRect = GetComponent<RectTransform>();

        // 初始化所有Buff图片为隐藏状态
        foreach (var img in buffImages)
        {
            img.gameObject.SetActive(false);
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
        }
    }

    private void Update()
    {
        // 获取ComboPresenter单例实例
        ComboPresenter comboPresenter = ComboPresenter.Instance;
        if (comboPresenter == null) return; // 确保实例已初始化

        // 获取触发的Buff列表（最多10个）
        var triggeredBuffs = comboPresenter.GetTriggeredBuffTypes();

        // 统计各buff类型被触发次数
        UpdateBuffCountsOnListChange(triggeredBuffs);

        // 关键调整：反转列表顺序，使最早触发的Buff排在最前面
        var orderedBuffs = triggeredBuffs.AsEnumerable().Reverse().ToList();

        // 去重并限制最多10个Buff（对应10个位置）
        var visibleBuffs = orderedBuffs.Distinct().Take(buffPositions.Length).ToList();

        // 显示触发的Buff图片
        for (int i = 0; i < visibleBuffs.Count; i++)
        {
            var buffType = visibleBuffs[i];
            if (!_buffIndexMap.TryGetValue(buffType, out int imgIndex))
            {
                continue;
            }

            // 超出图片列表范围时跳过
            if (imgIndex >= buffImages.Count)
            {
                continue;
            }

            var targetImg = buffImages[imgIndex];

            // 设置位置（注意坐标是相对于当前容器的）
            targetImg.rectTransform.anchoredPosition = buffPositions[i];

            // 直接设置图片为完全不透明并激活
            targetImg.color = new Color(targetImg.color.r, targetImg.color.g, targetImg.color.b, 1f);
            targetImg.gameObject.SetActive(true);
        }

        // 隐藏未激活的Buff图片
        foreach (var img in buffImages)
        {
            bool isActive = visibleBuffs.Exists(b => _buffIndexMap[b] == buffImages.IndexOf(img));

            if (!isActive)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);
                img.gameObject.SetActive(false);
            }
        }

        // 更新所有Buff文本显示
        UpdateBuffTextCounts();
    }

    // 当触发列表长度变化时重新计数的函数
    private void UpdateBuffCountsOnListChange(List<ComboPresenter.BuffType> currentBuffs)
    {
        // 检测列表长度是否变化
        if (currentBuffs.Count == _lastBuffListCount) return;

        // 重新统计当前列表中各Buff的出现次数（含重复）
        _currentBuffCounts = currentBuffs
            .GroupBy(buff => buff)
            .ToDictionary(
                group => group.Key,
                group => group.Count()
            );

        // 更新上一次记录的长度
        _lastBuffListCount = currentBuffs.Count;
    }

    // 更新Buff文本显示次数的核心函数
    private void UpdateBuffTextCounts()
    {
        // 遍历所有Buff类型映射
        foreach (var buffMapping in _buffIndexMap)
        {
            ComboPresenter.BuffType buffType = buffMapping.Key;
            int targetIndex = buffMapping.Value;

            // 检查文本组件是否存在
            if (targetIndex >= buffTexts.Count || buffTexts[targetIndex] == null)
            {
                continue;
            }

            // 获取当前Buff的触发次数（默认0次）
            _currentBuffCounts.TryGetValue(buffType, out int triggerCount);

            // 获取对应的文本组件
            TextMeshProUGUI targetText = buffTexts[targetIndex];

            // 设置文本内容（显示次数）
            targetText.text = triggerCount.ToString();

            // 根据对应图片的激活状态控制文本显示
            bool isImageActive = buffImages[targetIndex].gameObject.activeSelf;

            // 确保文本完全不透明
            targetText.alpha = 1f;
            targetText.gameObject.SetActive(isImageActive);
        }
    }
}