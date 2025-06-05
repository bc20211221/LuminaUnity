using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject bossPrefab; // Ԥ�Ƶ�Boss����
    public Transform spawnPoint; // Boss���ɵ�λ��
    public int enemiesToDefeat = 5; // ��Ҫ���ܶ��ٵ��˺����Boss
    public float spawnTimeAfterStart = 60f; // ��Ϸ��ʼ�󾭹�һ��ʱ���Boss����

    private int enemiesDefeated = 0;
    private bool bossSpawned = false;
    private float timer;

    // ����ģʽ��ȷ��ֻ��һ��ʵ��
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

    // �����˱�����ʱ�����ô˷���
    public void EnemyDefeated()
    {
        if (bossSpawned) return; // ���Boss�Ѿ����ɣ����ٴ���

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
            Debug.LogWarning("�޷�����Boss: ȱ��Ԥ��������ɵ㣬����Boss�Ѿ�����");
            return;
        }

        Instantiate(bossPrefab, spawnPoint.position, spawnPoint.rotation);
        bossSpawned = true;

        // �������������Boss���ֵ���Ч����Ч
        Debug.Log("Boss�����ɣ�");
    }
}