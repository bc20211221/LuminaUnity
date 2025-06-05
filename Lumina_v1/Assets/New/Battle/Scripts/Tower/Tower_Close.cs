using UnityEngine;

public class Tower_1 : MonoBehaviour
{
    public int maxHealth = 10;
    public int damage = 5;
    public float attackRange = 2f; // 缩短攻击范围，更适合近战
    public float attackCooldown = 2f;
    public float meleeRadius = 1f; // 近战攻击半径
    public GameObject meleeEffectPrefab; // 近战攻击效果预制体

    private int currentHealth;
    private float attackTimer;
    private HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            GameObject nearestEnemy = FindNearestEnemy();
            if (nearestEnemy != null && Vector2.Distance(transform.position, nearestEnemy.transform.position) <= attackRange)
            {
                MeleeAttack(nearestEnemy);
                attackTimer = 0f;
            }
        }
    }

    GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy;
            }
        }

        return nearest;
    }

    void MeleeAttack(GameObject target)
    {
        // 创建近战攻击效果
        if (meleeEffectPrefab != null)
        {
            GameObject effect = Instantiate(meleeEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 0.5f); // 短暂显示后销毁效果
        }

        // 检测范围内的所有敌人并造成伤害
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, meleeRadius);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                // 假设敌人有TakeDamage方法
                enemy.GetComponent<Enemy>()?.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    // 可视化近战攻击范围（仅在编辑器中可见）
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, meleeRadius);
    }
}