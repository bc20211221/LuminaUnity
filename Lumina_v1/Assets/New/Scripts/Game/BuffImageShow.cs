using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

using Game.Process;
using TMPro;

public class BuffImageShow : MonoBehaviour
{
    [Header("Buff对应图片组件")]
    [Tooltip("按顺序对应不同Buff的图片")]
    [SerializeField] private List<Image> buffImages = new List<Image>();

    [Header("Buff对应文本组件")]
    [Tooltip("按顺序对应不同Buff图片下的文本组件（需与buffImages顺序一致）")]
    [SerializeField] private List<TextMeshProUGUI> buffTexts = new List<TextMeshProUGUI>(); // 修改为TextMeshProUGUI

    [Header("显示参数")]
    [Tooltip("图片显示/隐藏的渐变时间（秒）")]
    [SerializeField] private float fadeDuration = 0.3f;

    private RectTransform _containerRect;

    // 预设10个坐标（5列2行）
    private Vector2[] buffPositions = new Vector2[]
    {
        new Vector2(810, 378), // 第一行第五列
        new Vector2(702, 378), // 第一行第四列
        new Vector2(594, 378), // 第一行第三列
        new Vector2(486, 378), // 第一行第二列
        new Vector2(378, 378), // 第一行第一列
        new Vector2(810, 271),  // 第二行第五列
        new Vector2(702, 271), // 第二行第四列
        new Vector2(594, 271), // 第二行第三列
        new Vector2(486, 271), // 第二行第二列
        new Vector2(378, 271)  // 第二行第一列
        /*new Vector2(378, 378), // 第一行第一列
        new Vector2(486, 378), // 第一行第二列
        new Vector2(594, 378), // 第一行第三列
        new Vector2(702, 378), // 第一行第四列
        new Vector2(810, 378), // 第一行第五列
        new Vector2(378, 271), // 第二行第一列
        new Vector2(486, 271), // 第二行第二列
        new Vector2(594, 271), // 第二行第三列
        new Vector2(702, 271), // 第二行第四列
        new Vector2(810, 271)  // 第二行第五列*/
        
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

        // 初始化所有TextMeshPro文本为隐藏状态
        foreach (var text in buffTexts)
        {
            if (text != null) // 安全检查
            {
                text.gameObject.SetActive(false);
                text.alpha = 0; // TextMeshPro使用alpha属性控制透明度
            }
        }
    }

    private void Update()
    {
        // 获取ComboPresenter单例实例
        ComboPresenter comboPresenter = ComboPresenter.Instance;
        if (comboPresenter == null) return; // 确保实例已初始化

        // 获取触发的Buff列表（最多10个）
        var triggeredBuffs = comboPresenter.GetTriggeredBuffTypes();

        //统计各buff类型被触发次数
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
                Debug.LogWarning($"未找到Buff类型 {buffType} 对应的图片索引");
                continue;
            }

            // 超出图片列表范围时跳过
            if (imgIndex >= buffImages.Count)
            {
                Debug.LogError($"图片列表索引越界：当前索引 {imgIndex}，列表长度 {buffImages.Count}");
                continue;
            }

            var targetImg = buffImages[imgIndex];
            // 设置位置（注意坐标是相对于当前容器的）
            targetImg.rectTransform.anchoredPosition = buffPositions[i];

            // 渐变显示
            if (targetImg.color.a < 1f)
            {
                targetImg.color = new Color(
                    targetImg.color.r,
                    targetImg.color.g,
                    targetImg.color.b,
                    Mathf.MoveTowards(targetImg.color.a, 1f, Time.deltaTime / fadeDuration)
                );
                targetImg.gameObject.SetActive(true);
            }
        }

        foreach (var img in buffImages)
        {
            bool isActive = visibleBuffs.Exists(b => _buffIndexMap[b] == buffImages.IndexOf(img));
            if (!isActive)
            {
                if (img.color.a > 0f)
                {
                    img.color = new Color(
                        img.color.r,
                        img.color.g,
                        img.color.b,
                        Mathf.MoveTowards(img.color.a, 0f, Time.deltaTime / fadeDuration)
                    );
                }
                else
                {
                    img.gameObject.SetActive(false);
                }
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

        // 输出调试信息
        string countStr = string.Join(" ", _currentBuffCounts.Select(kv => $"{kv.Key}:{kv.Value}"));
        Debug.Log($"触发列表长度变化（旧：{_lastBuffListCount} → 新：{currentBuffs.Count}），当前计数：{countStr}");

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
                Debug.LogError($"文本组件索引越界或为空：类型 {buffType} 对应索引 {targetIndex}，文本列表长度 {buffTexts.Count}");
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

            if (isImageActive)
            {
                // 图片显示时，文本渐变显示
                if (targetText.alpha < 1f)
                {
                    targetText.alpha = Mathf.MoveTowards(targetText.alpha, 1f, Time.deltaTime / fadeDuration);
                }
                targetText.gameObject.SetActive(true);
            }
            else
            {
                // 图片隐藏时，文本渐变隐藏
                if (targetText.alpha > 0f)
                {
                    targetText.alpha = Mathf.MoveTowards(targetText.alpha, 0f, Time.deltaTime / fadeDuration);
                }
                else
                {
                    targetText.gameObject.SetActive(false);
                }
            }
        }
    }
}


