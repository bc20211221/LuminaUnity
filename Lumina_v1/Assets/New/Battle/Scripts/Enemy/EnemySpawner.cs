using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 3f; // �������ɵ�ʱ����
    public int maxEnemies = 10; // ������ͬʱ���ڵ�����������
    public float spawnRadius = 7f; // ���ɵ��˵İ뾶
    public float startDelay = 5f; // ��Ϸ��ʼ���ӳ����ɵ�ʱ��

    private float timer; // ���ڼ�¼ʱ��
    private float delayTimer; // ���ڼ�¼�ӳ�ʱ��
    private int currentEnemies = 0; // ��ǰ�����еĵ�������

    void Start()
    {
        delayTimer = 0f; // ��ʼ���ӳټ�ʱ��
        timer = 0f; // ��ʼ�����ɼ�ʱ��
    }

    void Update()
    {
        // ����Ƿ��Ѿ������ӳ�ʱ��
        if (delayTimer < startDelay)
        {
            delayTimer += Time.deltaTime;
            return;
        }

        // ����Ƿ��������µ��˵�ʱ��
        timer += Time.deltaTime;
        if (timer >= spawnInterval && currentEnemies < maxEnemies)
        {
            SpawnEnemy();
            timer = 0f; // ���ü�ʱ��
        }
    }

    void SpawnEnemy()
    {
        // �ڰ뾶��Χ���������һ���Ƕ�
        float angle = Random.Range(0f, 180f);
        Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;
        // �ڰ뾶��Χ�����һ������
        float randomDistance = Random.Range(0f, spawnRadius);
        Vector2 spawnPos = (Vector2)transform.position + direction * randomDistance;

        // ʵ��������
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        currentEnemies++;

        // ����������ʱ���ٵ�ǰ�����еĵ�������
        enemy.GetComponent<EN>().onDeath += () => currentEnemies--;
    }
}