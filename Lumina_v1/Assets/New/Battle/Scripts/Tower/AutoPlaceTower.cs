using UnityEngine;
using Game.Process;
using UnityEngine.UI;
using System.Collections;
using System;

public class AutoPlaceTower : MonoBehaviour
{
    //��¼MISS����
    private int misscount = 0;
    private TowerPlacer towerPlacer;
    public static AutoPlaceTower Instance { get; private set; }


    [Header("����������")]
    [Tooltip("����������۵Ĳο�ͼƬ�����ڻ�ȡλ�ã�")]
    public Image[] laneEnergySlotReferenceImages; 

    [Tooltip("���й����������ͼƬ�������˳�����У�ÿ�����5�ţ�")]
    public Sprite[] allLaneEnergySprites; 

    [Header("��Ϸ����")]
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
            Debug.LogError($"�����۲ο�ͼƬ������� {laneCount} �������");
            enabled = false;
            return;
        }

        if (allLaneEnergySprites == null || allLaneEnergySprites.Length != laneCount * STAGES_PER_LANE)
        {
            Debug.LogError($"������ͼƬ���������� {laneCount * STAGES_PER_LANE} ��ͼƬ��");
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
            Debug.LogError("��Ч�Ĺ������");
            return;
        }

        laneHitCounts = new int[laneCount];
        laneToTowerType ??= new int[laneCount];

        if (laneToTowerType.Length != laneCount)
        {
            Array.Resize(ref laneToTowerType, laneCount);
            for (int i = 0; i < laneCount; i++) laneToTowerType[i] = i;
        }

        Debug.Log($"��ʼ�� {laneCount} �����");
    }


    public void OnNoteJudged(int result, int laneIndex)
    {
        //Debug.Log($"OnNoteJudged �����ã�result={result}, laneIndex={laneIndex}");
        //ͳ��MISS��������������������
        //if (result < 0 || !IsValidLaneIndex(laneIndex)) return;
        if (!IsValidLaneIndex(laneIndex)) return;
        if(result < 0)
        {
            misscount++;
            //Debug.Log($"��ǰ������������{BuffAttribute.BiologyDeath}MISS,��ǰMISS{misscount}");
            if(misscount >= BuffAttribute.BiologyDeath)
            {
                Debug.Log($"��ǰ������������{BuffAttribute.BiologyDeath}MISS");
                misscount=0;
                //�������һ��������
                towerPlacer.DestoryRandomTower();
            }
        }
        else
        {
            laneHitCounts[laneIndex]++;
            //Debug.Log($"��� {laneIndex} ���м���: {laneHitCounts[laneIndex]}");
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
            Debug.Log($"�ڹ�� {laneIndex} ���������� {towerType}");
        }
    }

    private void ValidateInitialization()
    {
        if (laneHitCounts == null || laneHitCounts.Length != laneCount)
        {
            Debug.LogError("������м���δ��ȷ��ʼ����");
        }

        if (laneEnergySlotContainers == null || laneEnergySlotContainers.Length != laneCount)
        {
            Debug.LogError("����������δ��ȷ��ʼ����");
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
            Debug.LogError($"��Ч�Ĺ������: {laneIndex} (���: {laneCount - 1})");
        }
        return isValid;
    }

    private int GetTowerTypeForLane(int laneIndex)
    {
        if (laneToTowerType == null || laneIndex >= laneToTowerType.Length)
        {
            Debug.LogWarning($"ʹ�ù�� {laneIndex} ��Ĭ��������");
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
            //Debug.Log($"�������� {i} ����Ϊ {towerType} ��������");
/*
            // ��ȡ�����͵�������ʵ��
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