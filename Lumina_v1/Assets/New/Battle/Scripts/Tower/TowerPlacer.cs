using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class TowerPlacer : MonoBehaviour
{
    [Header("ͨ������")]
    public int energy = 100;
    public GameObject[] towerPrefabs = new GameObject[4];
    public GameObject battleArea;
    public int arcSections = 3; 
    public float myRotationAngle = 90f;
    public float towerOffset = 0.5f;
    public Text Message;
    public GameObject MessagePanel;
    //��ʾ������������ʾ
    public Text TowerDiedText;
    public GameObject TowerDiedTextPanel;

    [Header("��Ȧ����")]
    public float innerRadius = 6f;                   
    public int maxTowersPerInnerSector = 5;        

    [Header("��Ȧ����")]
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
            Debug.LogError($"δ�ҵ���ɫID {characterId} ��Ӧ��������");
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
                //����
                int towerID= towerComponent.towerID;
    
                if (towerComponent != null)
                {
                    towerComponent.SetTowerPlacer(this);
                    towerComponent.SetTowerType(towerType);
                    towerTypeToInstances[towerType].Add(towerComponent);
                    //�������ɵ�����������
                    //towerComponent.TowerTypeIndex = towerType;

                }
                MessagePanel.SetActive(true);
                Message.text = "����[" +(
                towerID == 0 ? "ӫˮĸ" :
                towerID == 1 ? "ִ����" :
                towerID == 2 ? "ө���" :
                towerID == 3 ? "����" : 
                towerID == 4 ? "��⹽" : 
                towerID == 5 ? "����ө" : 
                "δ֪") + "]";
            Invoke("ClearMessage", 1f);

                int characterId = characterIdToTowerType.FirstOrDefault(x => x.Value == towerType).Key;
                //ShowMessage($"5 Combo [{GetTowerNameByCharacterId(characterId)}]");

                sectorCounts[currentSector] = towerCount + j + 1;
                return true;
            }
        }
        return false;
    }

    //�����������һ�����ķ���
    public void DestoryRandomTower()
    {
        if(towerToSectorMap.Count == 0)
        {
            Debug.Log("û�п����ٵ���");
            return;
        }
        //���ѡ��һ����
        List<GameObject> allTowers = new List<GameObject>(towerToSectorMap.Keys);
        int randomIndex = Random.Range(0,allTowers.Count);
        GameObject towerToDestory = allTowers[randomIndex];
        //��ȡ����Type����
        TO towerComponent = towerToDestory.GetComponent<TO>();
        if(towerComponent == null)
        {
            Debug.LogError("������ȱ��Tower���");
            return;
        }
        //int towerType = towerComponent.TowerTypeIndex;
        int towerDieID= towerComponent.towerID;
        string towerName = towerDieID == 0 ? "ӫˮĸ":
                                         towerDieID == 1 ? "ִ����":
                                         towerDieID == 2 ? "ө���":
                                         towerDieID == 3 ? "����":
                                         towerDieID == 4 ? "��⹽":
                                         towerDieID == 5 ? "����ө":
                                         "δ֪";
        //��ʾ������ʾ
        TowerDiedTextPanel.SetActive(true);
        TowerDiedText.text = $"{BuffAttribute.BiologyDeath} Miss ���˥�� [{towerName}]";
        Invoke("ClearDiedMessage", 1f);
        //������Ϸ����
        OnTowerDestroyed(towerToDestory);
        Destroy(towerToDestory);
        Debug.Log($"��������{towerToDestory.name}");

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
            Debug.LogError($"��ɫID���鳤��({characterIds.Length})��Ԥ�������鳤��({towerPrefabs.Length})��ƥ��");
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
                Debug.LogError($"δ�ҵ�IDΪ {characterIds[i]} ��Ԥ���壬ʹ��Ĭ��Ԥ����");
            }
        }
    }

    public void TriggerAttackByCharacterId(int characterId)
    {
        if (!characterIdToTowerType.TryGetValue(characterId, out int towerType))
        {
            Debug.LogError($"δ�ҵ���ɫID {characterId} ��Ӧ��������");
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
            0 => "��˪��ʦ",
            1 => "����ʹ��",
            2 => "��֮��ʦ",
            3 => "��Ӱ��ʦ",
            4 => "��Ӱ��ʦ2",
            5 => "��Ӱ��ʦ3",
            _ => "δ֪"
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

    //����������ʾ
    private void ClearDiedMessage()
    {
        TowerDiedText.text = "";
        TowerDiedTextPanel.SetActive(false);
    }
}