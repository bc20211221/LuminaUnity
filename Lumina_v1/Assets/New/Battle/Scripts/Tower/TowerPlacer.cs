using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class TowerPlacer : MonoBehaviour
{
    [Header("通用设置")]
    public int energy = 100;
    public GameObject[] towerPrefabs = new GameObject[4];
    public GameObject battleArea;
    public int arcSections = 3; 
    public float myRotationAngle = 90f;
    public float towerOffset = 0.5f;
    public Text Message;
    public GameObject MessagePanel;
    //显示生物塔死亡提示
    public Text TowerDiedText;
    public GameObject TowerDiedTextPanel;

    [Header("内圈设置")]
    public float innerRadius = 6f;                   
    public int maxTowersPerInnerSector = 5;        

    [Header("外圈设置")]
    public float outerRadius = 8f;                  
    public int maxTowersPerOuterSector = 10;          

    private Dictionary<int, int> innerTowersPerSector = new Dictionary<int, int>();
    private Dictionary<int, int> outerTowersPerSector = new Dictionary<int, int>();
    private Dictionary<GameObject, (int sector, bool isOuter)> towerToSectorMap = new Dictionary<GameObject, (int, bool)>();

    private Dictionary<int, List<TO>> towerTypeToInstances = new Dictionary<int, List<TO>>();
    private Dictionary<int, int> characterIdToTowerType = new Dictionary<int, int>();

    private List<Vector3> placedTowerPositions = new List<Vector3>();

    public float centerOffset = 3f;




    void Start()
    {
        for (int i = 0; i < towerPrefabs.Length; i++)
        {
            towerTypeToInstances[i] = new List<TO>();
        }
    }

    public void PlaceTower(int towerType)
    {
        if (towerType < 0 || towerType >= towerPrefabs.Length)
        {
            Debug.Log("Invalid tower type.");
            return;
        }

        float totalAngle = 120f;                 
        float startAngle = myRotationAngle - 90f - totalAngle / 2f;
        float anglePerSection = totalAngle / arcSections;

        bool[] circleOrder = Random.value < 0.5f ? new bool[] { false, true } : new bool[] { true, false };

        foreach (bool isOuter in circleOrder)
        {
            if (TryPlaceInCircle(towerType, isOuter, anglePerSection, startAngle))
            {
                return; 
            }
        }

        Debug.Log("Failed to place tower - all sectors are full");
    }

    public void PlaceTowerByCharacterId(int characterId)
    {
        if (!characterIdToTowerType.TryGetValue(characterId, out int towerType))
        {
            Debug.LogError($"未找到角色ID {characterId} 对应的塔类型");
            return;
        }
        PlaceTower(towerType);
    }

    private bool TryPlaceInCircle(int towerType, bool isOuterCircle, float anglePerSection, float startAngle)
    {
        float radius = isOuterCircle ? outerRadius : innerRadius;
        int maxTowers = isOuterCircle ? maxTowersPerOuterSector : maxTowersPerInnerSector;
        var sectorCounts = isOuterCircle ? outerTowersPerSector : innerTowersPerSector;

        int startSector = Random.Range(0, arcSections);
        for (int i = 0; i < arcSections; i++)
        {
            int currentSector = (startSector + i) % arcSections;

            if (sectorCounts.ContainsKey(currentSector) && sectorCounts[currentSector] >= maxTowers)
            {
                continue;
            }

            int towerCount = sectorCounts.GetValueOrDefault(currentSector, 0);
            for (int j = 0; j < maxTowers - towerCount; j++)
            {
                float angle = myRotationAngle + startAngle + currentSector * anglePerSection + anglePerSection / 2f;

                Vector3 basePosition = battleArea.transform.position +
                    Quaternion.Euler(0, 0, angle) * (Vector3.right * (radius - centerOffset));

                float offsetFactor = isOuterCircle ? 1f : -1f;
                Vector3 offset = Quaternion.Euler(0, 0, angle + 90) * Vector3.right *
                    (j * towerOffset * offsetFactor);

                Vector3 finalPosition = basePosition + offset;

                if (IsPositionOccupied(finalPosition))
                {
                    continue;
                }

                GameObject newTower = Instantiate(towerPrefabs[towerType], finalPosition, Quaternion.identity);

                towerToSectorMap[newTower] = (currentSector, isOuterCircle);
                placedTowerPositions.Add(finalPosition);
                TO towerComponent = newTower.GetComponent<TO>();
                //补充
                int towerID= towerComponent.towerID;
    
                if (towerComponent != null)
                {
                    towerComponent.SetTowerPlacer(this);
                    towerComponent.SetTowerType(towerType);
                    towerTypeToInstances[towerType].Add(towerComponent);
                    //保存生成的生物塔索引
                    //towerComponent.TowerTypeIndex = towerType;

                }
                MessagePanel.SetActive(true);
                Message.text = "唤醒[" +(
                towerID == 0 ? "荧水母" :
                towerID == 1 ? "执灯鱼" :
                towerID == 2 ? "萤火虫" :
                towerID == 3 ? "藻灵" : 
                towerID == 4 ? "雾光菇" : 
                towerID == 5 ? "蓝海萤" : 
                "未知") + "]";
            Invoke("ClearMessage", 1f);

                int characterId = characterIdToTowerType.FirstOrDefault(x => x.Value == towerType).Key;
                //ShowMessage($"5 Combo [{GetTowerNameByCharacterId(characterId)}]");

                sectorCounts[currentSector] = towerCount + j + 1;
                return true;
            }
        }
        return false;
    }

    //新增随机销毁一个塔的方法
    public void DestoryRandomTower()
    {
        if(towerToSectorMap.Count == 0)
        {
            Debug.Log("没有可销毁的塔");
            return;
        }
        //随机选择一个塔
        List<GameObject> allTowers = new List<GameObject>(towerToSectorMap.Keys);
        int randomIndex = Random.Range(0,allTowers.Count);
        GameObject towerToDestory = allTowers[randomIndex];
        //获取塔的Type索引
        TO towerComponent = towerToDestory.GetComponent<TO>();
        if(towerComponent == null)
        {
            Debug.LogError("塔对象缺少Tower组件");
            return;
        }
        //int towerType = towerComponent.TowerTypeIndex;
        int towerDieID= towerComponent.towerID;
        string towerName = towerDieID == 0 ? "荧水母":
                                         towerDieID == 1 ? "执灯鱼":
                                         towerDieID == 2 ? "萤火虫":
                                         towerDieID == 3 ? "藻灵":
                                         towerDieID == 4 ? "雾光菇":
                                         towerDieID == 5 ? "蓝海萤":
                                         "未知";
        //显示销毁提示
        TowerDiedTextPanel.SetActive(true);
        TowerDiedText.text = $"{BuffAttribute.BiologyDeath} Miss 随机衰败 [{towerName}]";
        Invoke("ClearDiedMessage", 1f);
        //销毁游戏对象
        OnTowerDestroyed(towerToDestory);
        Destroy(towerToDestory);
        Debug.Log($"已销毁塔{towerToDestory.name}");

    }

    private bool IsPositionOccupied(Vector3 position)
    {
        foreach (Vector3 placedPosition in placedTowerPositions)
        {
            if (Vector3.Distance(position, placedPosition) < towerOffset * 0.3f) 
            {
                return true;
            }
        }
        return false;
    }

    public void OnTowerDestroyed(GameObject tower)
    {
        if (tower == null) return;

        if (towerToSectorMap.TryGetValue(tower, out (int sector, bool isOuter) sectorInfo))
        {
            var sectorCounts = sectorInfo.isOuter ? outerTowersPerSector : innerTowersPerSector;
            if (sectorCounts.TryGetValue(sectorInfo.sector, out int count))
            {
                count--;
                if (count <= 0) sectorCounts.Remove(sectorInfo.sector);
                else sectorCounts[sectorInfo.sector] = count;
            }
            towerToSectorMap.Remove(tower);
        }

        TO towerComponent = tower.GetComponent<TO>();
        if (towerComponent != null && towerTypeToInstances.ContainsKey(towerComponent.TowerType))
        {
            towerTypeToInstances[towerComponent.TowerType].Remove(towerComponent);

        }

        Vector3 towerPosition = tower.transform.position;
        if (placedTowerPositions.Contains(towerPosition))
        {
            placedTowerPositions.Remove(towerPosition);
        }

        Debug.Log("Tower destroyed");
    }

    public void UpdateTowerPrefabsByCharacterIds(int[] characterIds)
    {
        if (characterIds.Length != towerPrefabs.Length)
        {
            Debug.LogError($"角色ID数组长度({characterIds.Length})和预制体数组长度({towerPrefabs.Length})不匹配");
            return;
        }

        characterIdToTowerType.Clear();

        for (int i = 0; i < characterIds.Length; i++)
        {
            GameObject prefab = TowerDataManager.Instance.GetTowerPrefabById(characterIds[i]);
            if (prefab != null)
            {
                towerPrefabs[i] = prefab;
                characterIdToTowerType[characterIds[i]] = i; 
            }
            else
            {
                Debug.LogError($"未找到ID为 {characterIds[i]} 的预制体，使用默认预制体");
            }
        }
    }

    public void TriggerAttackByCharacterId(int characterId)
    {
        if (!characterIdToTowerType.TryGetValue(characterId, out int towerType))
        {
            Debug.LogError($"未找到角色ID {characterId} 对应的塔类型");
            return;
        }

        TriggerAttackByTowerType(towerType);
    }

    public void TriggerAttackByTowerType(int towerType)
    {
        if (!towerTypeToInstances.ContainsKey(towerType))
        {
            Debug.Log($"No towers of type {towerType} found");
            return;
        }

        foreach (TO tower in towerTypeToInstances[towerType])
        {
            if (tower != null && !tower.IsDead())
            {
                tower.ForceAttack();
            }
        }
    }

    private int SelectSector(HashSet<int> triedSectors, int attempt)
    {
        if (attempt == 0)
        {
            return Random.Range(0, arcSections);
        }
        else
        {
            List<int> availableSectors = new List<int>();
            for (int i = 0; i < arcSections; i++)
            {
                if (!triedSectors.Contains(i))
                {
                    availableSectors.Add(i);
                }
            }

            return availableSectors.Count > 0 ?
                availableSectors[Random.Range(0, availableSectors.Count)] :
                -1;
        }
    }

    private string GetTowerNameByCharacterId(int characterId)
    {
        var config = TowerDataManager.Instance.towerPrefabConfig;
        if (config != null)
        {
            foreach (var entry in config.towerPrefabs)
            {
                if (entry.id == characterId)
                {
                    return entry.towerName;
                }
            }
        }

        return GetTowerName(characterIdToTowerType.GetValueOrDefault(characterId, 0));
    }

    private string GetTowerName(int towerType)
    {
        return towerType switch
        {
            0 => "冰霜法师",
            1 => "火焰使者",
            2 => "光之法师",
            3 => "暗影法师",
            4 => "暗影法师2",
            5 => "暗影法师3",
            _ => "未知"
        };
    }

    private void ShowMessage(string message)
    {
        Message.text = message;
        MessagePanel.SetActive(true);
        Invoke("ClearMessage", 1f);
    }

    private void ClearMessage()
    {
        Message.text = "";
        MessagePanel.SetActive(false);
    }

    //新增销毁提示
    private void ClearDiedMessage()
    {
        TowerDiedText.text = "";
        TowerDiedTextPanel.SetActive(false);
    }
}