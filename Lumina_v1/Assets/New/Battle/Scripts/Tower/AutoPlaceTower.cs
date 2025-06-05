using UnityEngine;
using Game.Process;
using UnityEngine.UI;
using System.Collections;
using System;

public class AutoPlaceTower : MonoBehaviour
{
    //记录MISS次数
    private int misscount = 0;
    private TowerPlacer towerPlacer;
    public static AutoPlaceTower Instance { get; private set; }


    [Header("能量槽配置")]
    [Tooltip("各轨道能量槽的参考图片（用于获取位置）")]
    public Image[] laneEnergySlotReferenceImages; 

    [Tooltip("所有轨道的能量槽图片（按轨道顺序排列，每个轨道5张）")]
    public Sprite[] allLaneEnergySprites; 

    [Header("游戏设置")]
    public int laneCount = 4; 
    private Image[] laneEnergySlotContainers; 
    private int[] laneHitCounts;
    [SerializeField] private int[] laneToTowerType;



    private const int STAGES_PER_LANE = 5;
    private const float CLEAR_DELAY = 0.5f; 
    private Coroutine[] clearCoroutines; 

    public HitAnimationManager hitAnimationManager; 


    void Awake()
    {
        Instance = this;
        towerPlacer = FindObjectOfType<TowerPlacer>();
        clearCoroutines = new Coroutine[laneCount]; 

        if (laneEnergySlotReferenceImages == null || laneEnergySlotReferenceImages.Length != laneCount)
        {
            Debug.LogError($"能量槽参考图片必须包含 {laneCount} 个轨道！");
            enabled = false;
            return;
        }

        if (allLaneEnergySprites == null || allLaneEnergySprites.Length != laneCount * STAGES_PER_LANE)
        {
            Debug.LogError($"能量槽图片数组必须包含 {laneCount * STAGES_PER_LANE} 张图片！");
            enabled = false;
            return;
        }

        InitializeLaneData();
        CreateEnergySlots();
    }

    void Start()
    {
        ValidateInitialization();
        InitializeTowerTypeMapping();
    }

    private void CreateEnergySlots()
    {
        laneEnergySlotContainers = new Image[laneCount];

        for (int i = 0; i < laneCount; i++)
        {
            GameObject slotContainer = new GameObject($"Lane{i}EnergySlot");
            RectTransform referenceRect = laneEnergySlotReferenceImages[i].GetComponent<RectTransform>();
            slotContainer.transform.SetParent(referenceRect.parent, false);
            slotContainer.transform.localPosition = referenceRect.localPosition;
            slotContainer.transform.localRotation = referenceRect.localRotation;

            Image containerImage = slotContainer.AddComponent<Image>();
            containerImage.enabled = false;

            RectTransform containerRect = containerImage.GetComponent<RectTransform>();
            containerRect.sizeDelta = referenceRect.sizeDelta;
            containerRect.anchorMin = referenceRect.anchorMin;
            containerRect.anchorMax = referenceRect.anchorMax;
            containerRect.pivot = referenceRect.pivot;

            laneEnergySlotContainers[i] = containerImage;
        }
    }

    private void InitializeLaneData()
    {
        if (laneCount <= 0)
        {
            Debug.LogError("无效的轨道数！");
            return;
        }

        laneHitCounts = new int[laneCount];
        laneToTowerType ??= new int[laneCount];

        if (laneToTowerType.Length != laneCount)
        {
            Array.Resize(ref laneToTowerType, laneCount);
            for (int i = 0; i < laneCount; i++) laneToTowerType[i] = i;
        }

        Debug.Log($"初始化 {laneCount} 条轨道");
    }


    public void OnNoteJudged(int result, int laneIndex)
    {
        //Debug.Log($"OnNoteJudged 被调用，result={result}, laneIndex={laneIndex}");
        //统计MISS次数并触发生物塔死亡
        //if (result < 0 || !IsValidLaneIndex(laneIndex)) return;
        if (!IsValidLaneIndex(laneIndex)) return;
        if(result < 0)
        {
            misscount++;
            //Debug.Log($"当前生物死亡条件{BuffAttribute.BiologyDeath}MISS,当前MISS{misscount}");
            if(misscount >= BuffAttribute.BiologyDeath)
            {
                Debug.Log($"当前生物死亡条件{BuffAttribute.BiologyDeath}MISS");
                misscount=0;
                //随机死亡一个生物塔
                towerPlacer.DestoryRandomTower();
            }
        }
        else
        {
            laneHitCounts[laneIndex]++;
            //Debug.Log($"轨道 {laneIndex} 命中计数: {laneHitCounts[laneIndex]}");
            UpdateEnergySlotImage(laneIndex);

            if (laneHitCounts[laneIndex] >= STAGES_PER_LANE)
            {
                if (clearCoroutines[laneIndex] != null) StopCoroutine(clearCoroutines[laneIndex]);
                clearCoroutines[laneIndex] = StartCoroutine(ProcessFullEnergySlot(laneIndex));
            }

            if (hitAnimationManager)
            {
                hitAnimationManager.PlayAnimation(laneIndex);
            }

            TriggerTowerAttack();
        } 

    }


    private IEnumerator ProcessFullEnergySlot(int laneIndex)
    {
        UpdateEnergySlotImage(laneIndex);

        yield return new WaitForSeconds(CLEAR_DELAY);

        laneHitCounts[laneIndex] = 0;
        UpdateEnergySlotImage(laneIndex);

        SpawnTower(laneIndex);
    }

    private void UpdateEnergySlotImage(int laneIndex)
    {
        if (!IsValidLaneIndex(laneIndex) || laneEnergySlotContainers[laneIndex] == null) return;

        int stage = Mathf.Clamp(laneHitCounts[laneIndex], 0, STAGES_PER_LANE);
        bool isActive = stage > 0;
        laneEnergySlotContainers[laneIndex].enabled = isActive;

        if (isActive)
        {
            int spriteIndex = GetSpriteIndex(laneIndex, stage - 1);
            laneEnergySlotContainers[laneIndex].sprite = allLaneEnergySprites[spriteIndex];
        }
    }

    private int GetSpriteIndex(int laneIndex, int stage)
    {
        return laneIndex * STAGES_PER_LANE + stage;
    }

    private void SpawnTower(int laneIndex)
    {
        int towerType = GetTowerTypeForLane(laneIndex);
        if (towerPlacer != null)
        {
            towerPlacer.PlaceTower(towerType);
            Debug.Log($"在轨道 {laneIndex} 放置塔类型 {towerType}");
        }
    }

    private void ValidateInitialization()
    {
        if (laneHitCounts == null || laneHitCounts.Length != laneCount)
        {
            Debug.LogError("轨道命中计数未正确初始化！");
        }

        if (laneEnergySlotContainers == null || laneEnergySlotContainers.Length != laneCount)
        {
            Debug.LogError("能量槽容器未正确初始化！");
        }
    }

    private void InitializeTowerTypeMapping()
    {
        if (laneToTowerType == null) return;

        for (int i = 0; i < laneToTowerType.Length; i++)
        {
            if (laneToTowerType[i] < 0)
            {
                laneToTowerType[i] = i;
            }
        }
    }

    private bool IsValidLaneIndex(int laneIndex)
    {
        bool isValid = laneIndex >= 0 && laneIndex < laneCount;
        if (!isValid)
        {
            Debug.LogError($"无效的轨道索引: {laneIndex} (最大: {laneCount - 1})");
        }
        return isValid;
    }

    private int GetTowerTypeForLane(int laneIndex)
    {
        if (laneToTowerType == null || laneIndex >= laneToTowerType.Length)
        {
            Debug.LogWarning($"使用轨道 {laneIndex} 的默认塔类型");
            return laneIndex % 4; 
        }

        return laneToTowerType[laneIndex];
    }

    private void TriggerTowerAttack()
    {

        if (towerPlacer == null) return;

        for (int i = 0; i < laneCount; i++)
        {
            int towerType = GetTowerTypeForLane(i);
            towerPlacer.TriggerAttackByTowerType(towerType);
            //Debug.Log($"触发车道 {i} 类型为 {towerType} 的塔攻击");
/*
            // 获取该类型的所有塔实例
            var towerInstances = towerPlacer.GetTowerTypeToInstances()[towerType];
            foreach (var tower in towerInstances)
            {
                var to = tower.GetComponent<TO>();
                if (to != null)
                {
                    to.ForceAttack();
                }
            }*/
        }
    }
}