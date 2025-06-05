using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject bossPrefab; // 预制的Boss对象
    public Transform spawnPoint; // Boss生成的位置
    public int enemiesToDefeat = 5; // 需要击败多少敌人后出现Boss
    public float spawnTimeAfterStart = 60f; // 游戏开始后经过一定时间后Boss出现

    private int enemiesDefeated = 0;
    private bool bossSpawned = false;
    private float timer;

    // 单例模式，确保只有一个实例
    public static BossSpawner Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        timer = 0f;
    }

    private void Update()
    {
        if (bossSpawned) return;

        timer += Time.deltaTime;
        if (timer >= spawnTimeAfterStart)
        {
            SpawnBoss();
        }
    }

    // 当敌人被击败时，调用此方法
    public void EnemyDefeated()
    {
        if (bossSpawned) return; // 如果Boss已经生成，不再处理

        enemiesDefeated++;

        if (enemiesDefeated >= enemiesToDefeat)
        {
            SpawnBoss();
        }
    }

    private void SpawnBoss()
    {
        if (bossPrefab == null || spawnPoint == null || bossSpawned)
        {
            Debug.LogWarning("无法生成Boss: 缺少预制体或生成点，或者Boss已经生成");
            return;
        }

        Instantiate(bossPrefab, spawnPoint.position, spawnPoint.rotation);
        bossSpawned = true;

        // 可以在这里添加Boss出现的音效和特效
        Debug.Log("Boss已生成！");
    }
}