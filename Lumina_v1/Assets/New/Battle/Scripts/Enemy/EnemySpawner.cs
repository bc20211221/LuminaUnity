using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 3f; // 敌人生成的时间间隔
    public int maxEnemies = 10; // 场景中同时存在的最大敌人数量
    public float spawnRadius = 7f; // 生成敌人的半径
    public float startDelay = 5f; // 游戏开始后延迟生成的时间

    private float timer; // 用于记录时间
    private float delayTimer; // 用于记录延迟时间
    private int currentEnemies = 0; // 当前场景中的敌人数量

    void Start()
    {
        delayTimer = 0f; // 初始化延迟计时器
        timer = 0f; // 初始化生成计时器
    }

    void Update()
    {
        // 检查是否已经过了延迟时间
        if (delayTimer < startDelay)
        {
            delayTimer += Time.deltaTime;
            return;
        }

        // 检查是否到了生成新敌人的时间
        timer += Time.deltaTime;
        if (timer >= spawnInterval && currentEnemies < maxEnemies)
        {
            SpawnEnemy();
            timer = 0f; // 重置计时器
        }
    }

    void SpawnEnemy()
    {
        // 在半径范围内随机生成一个角度
        float angle = Random.Range(0f, 180f);
        Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;
        // 在半径范围内随机一个距离
        float randomDistance = Random.Range(0f, spawnRadius);
        Vector2 spawnPos = (Vector2)transform.position + direction * randomDistance;

        // 实例化敌人
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        currentEnemies++;

        // 当敌人死亡时减少当前场景中的敌人数量
        enemy.GetComponent<EN>().onDeath += () => currentEnemies--;
    }
}