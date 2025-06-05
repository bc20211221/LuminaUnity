using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Animation Parameters")]
    public Animator animator;
    public bool isExist = false;
    public bool isDie = false;

    public int maxHealth = 10;
    public int damage = 5;
    public float attackRange = 4f;
    public GameObject projectilePrefab;

    private int currentHealth;
    private TowerPlacer towerPlacer;
    private int towerType; // 新增：塔的类型

    // 新增：只读属性，判断塔是否已死亡
    public bool IsDead => isDie;
    public int TowerType => towerType;

    void Start()
    {
        isDie = false;
        currentHealth = maxHealth;

        // 获取子对象中的血条组件
        HealthBar healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }

        // 初始化动画和碰撞器
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator != null)
        {
            animator.SetTrigger("Appear");
        }

        gameObject.AddComponent<BoxCollider2D>();
    }

    // 移除Update中的自动攻击逻辑

    GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance && distance <= attackRange)
            {
                minDistance = distance;
                nearest = enemy;
            }
        }

        return nearest;
    }

    // 强制攻击方法（由外部触发）
    public void ForceAttack()
    {
        if (isDie) return;

        GameObject nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            Attack(nearestEnemy);
        }
    }

    void Attack(GameObject target)
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // 生成弹射物并设置目标和伤害
        if (projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile proj = projectile.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.SetTarget(target.transform);
                proj.SetDamage(damage);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDie) return;

        currentHealth -= amount;

        // 更新血条
        HealthBar healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 设置TowerPlacer引用
    public void SetTowerPlacer(TowerPlacer placer)
    {
        towerPlacer = placer;
    }

    // 新增：设置塔的类型
    public void SetTowerType(int type)
    {
        towerType = type;
    }

    void Die()
    {
        if (isDie) return;

        isDie = true;

        // 通知管理器
        if (towerPlacer != null)
        {
            towerPlacer.OnTowerDestroyed(gameObject);
        }

        // 处理视觉效果和销毁
        if (animator != null)
        {
            animator.SetBool("IsDie", true);
        }

        // 禁用碰撞器
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // 延迟销毁对象
        Destroy(gameObject, 1.5f);
    }
}