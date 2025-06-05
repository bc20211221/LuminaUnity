using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

using Game.Process;
using TMPro;

public class BuffShow : MonoBehaviour
{
    [Header("Buff��ӦͼƬ���")]
    [Tooltip("��˳���Ӧ��ͬBuff��ͼƬ")]
    [SerializeField] private List<Image> buffImages = new List<Image>();

    [Header("Buff��Ӧ�ı����")]
    [Tooltip("��˳���Ӧ��ͬBuffͼƬ�µ��ı����������buffImages˳��һ�£�")]
    [SerializeField] private List<TextMeshProUGUI> buffTexts = new List<TextMeshProUGUI>(); // �޸�ΪTextMeshProUGUI

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

    // ��¼��һ�δ����б�ĳ��ȣ����ڼ��仯��
    private int _lastBuffListCount = 0;

    // �洢��ǰ��Buff���͵ĳ��ִ���������Buff���ͣ�ֵ��������
    private Dictionary<ComboPresenter.BuffType, int> _currentBuffCounts = new();

    private void Awake()
    {
        _containerRect = GetComponent<RectTransform>();

        // ��ʼ������BuffͼƬΪ����״̬
        foreach (var img in buffImages)
        {
            img.gameObject.SetActive(false);
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
        }
    }

    private void Update()
    {
        // ��ȡComboPresenter����ʵ��
        ComboPresenter comboPresenter = ComboPresenter.Instance;
        if (comboPresenter == null) return; // ȷ��ʵ���ѳ�ʼ��

        // ��ȡ������Buff�б����10����
        var triggeredBuffs = comboPresenter.GetTriggeredBuffTypes();

        // ͳ�Ƹ�buff���ͱ���������
        UpdateBuffCountsOnListChange(triggeredBuffs);

        // �ؼ���������ת�б�˳��ʹ���紥����Buff������ǰ��
        var orderedBuffs = triggeredBuffs.AsEnumerable().Reverse().ToList();

        // ȥ�ز��������10��Buff����Ӧ10��λ�ã�
        var visibleBuffs = orderedBuffs.Distinct().Take(buffPositions.Length).ToList();

        // ��ʾ������BuffͼƬ
        for (int i = 0; i < visibleBuffs.Count; i++)
        {
            var buffType = visibleBuffs[i];
            if (!_buffIndexMap.TryGetValue(buffType, out int imgIndex))
            {
                continue;
            }

            // ����ͼƬ�б�Χʱ����
            if (imgIndex >= buffImages.Count)
            {
                continue;
            }

            var targetImg = buffImages[imgIndex];

            // ����λ�ã�ע������������ڵ�ǰ�����ģ�
            targetImg.rectTransform.anchoredPosition = buffPositions[i];

            // ֱ������ͼƬΪ��ȫ��͸��������
            targetImg.color = new Color(targetImg.color.r, targetImg.color.g, targetImg.color.b, 1f);
            targetImg.gameObject.SetActive(true);
        }

        // ����δ�����BuffͼƬ
        foreach (var img in buffImages)
        {
            bool isActive = visibleBuffs.Exists(b => _buffIndexMap[b] == buffImages.IndexOf(img));

            if (!isActive)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);
                img.gameObject.SetActive(false);
            }
        }

        // ��������Buff�ı���ʾ
        UpdateBuffTextCounts();
    }

    // �������б��ȱ仯ʱ���¼����ĺ���
    private void UpdateBuffCountsOnListChange(List<ComboPresenter.BuffType> currentBuffs)
    {
        // ����б����Ƿ�仯
        if (currentBuffs.Count == _lastBuffListCount) return;

        // ����ͳ�Ƶ�ǰ�б��и�Buff�ĳ��ִ��������ظ���
        _currentBuffCounts = currentBuffs
            .GroupBy(buff => buff)
            .ToDictionary(
                group => group.Key,
                group => group.Count()
            );

        // ������һ�μ�¼�ĳ���
        _lastBuffListCount = currentBuffs.Count;
    }

    // ����Buff�ı���ʾ�����ĺ��ĺ���
    private void UpdateBuffTextCounts()
    {
        // ��������Buff����ӳ��
        foreach (var buffMapping in _buffIndexMap)
        {
            ComboPresenter.BuffType buffType = buffMapping.Key;
            int targetIndex = buffMapping.Value;

            // ����ı�����Ƿ����
            if (targetIndex >= buffTexts.Count || buffTexts[targetIndex] == null)
            {
                continue;
            }

            // ��ȡ��ǰBuff�Ĵ���������Ĭ��0�Σ�
            _currentBuffCounts.TryGetValue(buffType, out int triggerCount);

            // ��ȡ��Ӧ���ı����
            TextMeshProUGUI targetText = buffTexts[targetIndex];

            // �����ı����ݣ���ʾ������
            targetText.text = triggerCount.ToString();

            // ���ݶ�ӦͼƬ�ļ���״̬�����ı���ʾ
            bool isImageActive = buffImages[targetIndex].gameObject.activeSelf;

            // ȷ���ı���ȫ��͸��
            targetText.alpha = 1f;
            targetText.gameObject.SetActive(isImageActive);
        }
    }
}