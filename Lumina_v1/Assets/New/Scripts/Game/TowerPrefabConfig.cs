using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TowerPrefabConfig", menuName = "Game Config/Tower Prefab Config")]
public class TowerPrefabConfig : ScriptableObject
{
    [System.Serializable]
    public class TowerPrefabEntry
    {
        public int id;
        public string towerName;
        public GameObject prefab;
    }

    public List<TowerPrefabEntry> towerPrefabs = new List<TowerPrefabEntry>();

    // ��ȡ���IDֵ�������Զ�������ID
    public int GetMaxId()
    {
        int maxId = 0;
        foreach (var entry in towerPrefabs)
        {
            if (entry.id > maxId)
                maxId = entry.id;
        }
        return maxId;
    }

    // ���ID�Ƿ��Ѵ���
    public bool IsIdExists(int id)
    {
        foreach (var entry in towerPrefabs)
        {
            if (entry.id == id)
                return true;
        }
        return false;
    }
}