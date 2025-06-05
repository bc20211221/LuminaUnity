using UnityEngine;
using System.Collections.Generic;
using System;

public class TowerDataManager : MonoBehaviour
{
    public static TowerDataManager Instance { get; private set; }

    private Dictionary<int, GameObject> idToTowerPrefabMap = new Dictionary<int, GameObject>();
    public TowerPrefabConfig towerPrefabConfig; // �����ļ�����
    private TowerPlacer towerPlacer;
    private int[] selectedCharacterIds;
    // ����������ֻ�����ԣ���¶��ѡ��Ľ�ɫID���飨����Ϊnull��
    public int[] SelectedCharacterIds => selectedCharacterIds;
    public static event Action OnCharacterIdsLoaded; // �����¼�


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFromConfig(); // �Զ��������ļ���ʼ��
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        towerPlacer = FindObjectOfType<TowerPlacer>();

        // �� CharacterSelectionData ��ȡ��ѡ��Ľ�ɫ ID
        selectedCharacterIds = CharacterSelectionData.Instance.GetSelectedCharacterIds();
        string idString = string.Join(",",selectedCharacterIds);
        Debug.Log($"��ѡ��Ľ�ɫ ID:{idString}");
        string IdString = string.Join(",",SelectedCharacterIds);
        Debug.Log($"��ѡ��Ľ�ɫ ID:{IdString}");
        OnCharacterIdsLoaded?.Invoke(); // ���ݼ�����ɺ󴥷��¼�


        // ������Ԥ����
        towerPlacer.UpdateTowerPrefabsByCharacterIds(selectedCharacterIds);
    }

    // �������ļ���ʼ��ӳ���ϵ
    public void InitializeFromConfig()
    {
        if (towerPrefabConfig == null)
        {
            Debug.LogError("δ�ҵ���Ԥ���������ļ���");
            return;
        }

        idToTowerPrefabMap.Clear();
        foreach (var entry in towerPrefabConfig.towerPrefabs)
        {
            if (!idToTowerPrefabMap.ContainsKey(entry.id))
            {
                idToTowerPrefabMap.Add(entry.id, entry.prefab);
            }
            else
            {
                Debug.LogError($"�ظ��� ID: {entry.id}������: {entry.towerName}");
            }
        }
    }

    // ���� ID ��ȡ��Ԥ����
    public GameObject GetTowerPrefabById(int id)
    {
        if (idToTowerPrefabMap.TryGetValue(id, out GameObject prefab))
        {
            return prefab;
        }

        Debug.LogError($"δ�ҵ� ID Ϊ {id} ����Ԥ����");
        return null;
    }

    // ��ȡ���п��õ��� ID
    public List<int> GetAllTowerIds()
    {
        return new List<int>(idToTowerPrefabMap.Keys);
    }
}