using UnityEngine;
using System.Collections.Generic;
using System;

public class TowerDataManager : MonoBehaviour
{
    public static TowerDataManager Instance { get; private set; }

    private Dictionary<int, GameObject> idToTowerPrefabMap = new Dictionary<int, GameObject>();
    public TowerPrefabConfig towerPrefabConfig; // 配置文件引用
    private TowerPlacer towerPlacer;
    private int[] selectedCharacterIds;
    // 新增：公共只读属性，暴露已选择的角色ID数组（可能为null）
    public int[] SelectedCharacterIds => selectedCharacterIds;
    public static event Action OnCharacterIdsLoaded; // 新增事件


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFromConfig(); // 自动从配置文件初始化
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        towerPlacer = FindObjectOfType<TowerPlacer>();

        // 从 CharacterSelectionData 获取已选择的角色 ID
        selectedCharacterIds = CharacterSelectionData.Instance.GetSelectedCharacterIds();
        string idString = string.Join(",",selectedCharacterIds);
        Debug.Log($"已选择的角色 ID:{idString}");
        string IdString = string.Join(",",SelectedCharacterIds);
        Debug.Log($"已选择的角色 ID:{IdString}");
        OnCharacterIdsLoaded?.Invoke(); // 数据加载完成后触发事件


        // 更新塔预制体
        towerPlacer.UpdateTowerPrefabsByCharacterIds(selectedCharacterIds);
    }

    // 从配置文件初始化映射关系
    public void InitializeFromConfig()
    {
        if (towerPrefabConfig == null)
        {
            Debug.LogError("未找到塔预制体配置文件！");
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
                Debug.LogError($"重复的 ID: {entry.id}，塔名: {entry.towerName}");
            }
        }
    }

    // 根据 ID 获取塔预制体
    public GameObject GetTowerPrefabById(int id)
    {
        if (idToTowerPrefabMap.TryGetValue(id, out GameObject prefab))
        {
            return prefab;
        }

        Debug.LogError($"未找到 ID 为 {id} 的塔预制体");
        return null;
    }

    // 获取所有可用的塔 ID
    public List<int> GetAllTowerIds()
    {
        return new List<int>(idToTowerPrefabMap.Keys);
    }
}