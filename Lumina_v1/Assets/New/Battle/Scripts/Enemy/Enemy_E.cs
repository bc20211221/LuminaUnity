using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    public int maxHealth = 20;
    public int damage = 2;
    public float moveSpeed = 2f;
    public float attackRange = 5f; // 增加攻击范围
    public float attackCooldown = 1f;
    public Projectile projectilePrefab; // 子弹预制体
    public Transform firePoint; // 子弹发射点

    private int currentHealth;
    private float attackTimer;
    private Transform target;
    private HealthBar healthBar;

    public System.Action onDeath;
    private bool isAttacking = false;
    [Header("Animation Parameters")]
    public Animator animator;
    public bool isDie = false;

    void Start()
    {
        currentHealth = maxHealth;
        FindNearestTarget();

        // 初始化血条
        healthBar = GetComponentInChildren<HealthBar>();
        healthBar.SetMaxHealth(maxHealth);

        animator = GetComponent<Animator>();
        gameObject.AddComponent<CircleCollider2D>();
        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
    }

    void Update()
    {
        // 如果当前目标不存在，寻找新目标
        if (target == null)
        {
            FindNearestTarget();
            if (target == null) return; // 如果仍然没有目标，退出
        }

        // 移动逻辑 - 保持在攻击范围内
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        if (distanceToTarget > attackRange && !isAttacking)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
        else if (distanceToTarget < attackRange * 0.8f)
        {
            // 如果太近，后退一点
            Vector2 direction = (transform.position - target.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime * 0.5f);
        }

        // 朝向目标
        if (target != null)
        {
            Vector2 direction = target.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        // 攻击逻辑
        attackTimer += Time.deltaTime;
        if (distanceToTarget <= attackRange && attackTimer >= attackCooldown)
        {
            Attack();
            attackTimer = 0f;
        }
    }

    void FindNearestTarget()
    {
        // 优先寻找塔
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        float nearestDistance = Mathf.Infinity;
        Transform nearestTower = null;

        foreach (GameObject tower in towers)
        {
            float distance = Vector2.Distance(transform.position, tower.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTower = tower.transform;
            }
        }

        // 如果找到塔，设为目标
        if (nearestTower != null)
        {
            target = nearestTower;
        }
        else
        {
            // 没有塔则攻击总部
            GameObject hq = GameObject.FindGameObjectWithTag("Headquarters");
            if (hq != null) target = hq.transform;
        }
    }

    void Attack()
    {
        // 触发攻击动画
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // 发射子弹
        if (projectilePrefab != null && target != null)
        {
            Projectile projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            //projectile.damage = damage;
           // projectile.target = target;
            projectile.speed = 5f; // 设置子弹速度
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // 通知Boss生成器敌人被击败
        if (BossSpawner.Instance != null)
        {
            BossSpawner.Instance.EnemyDefeated();
        }

        isDie = true;
        isAttacking = true; // 停止移动
        BattleScoreBoard.Instance.AddScore(20);

        // 触发死亡动画
        if (animator != null)
        {
            animator.SetBool("IsDie", true);
        }

        // 禁用碰撞器，防止死后还能被攻击
        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
        onDeath?.Invoke();
        // 延迟销毁对象，让死亡动画有足够时间播放
        float destroyDelay = 1.5f; // 根据你的死亡动画长度调整
        Destroy(gameObject, destroyDelay);
    }

    // 碰撞检测
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Headquarters"))
        {
            target = other.transform; // 确保目标是 Headquarters
            var hq = other.GetComponent<Headquarters>();
            if (hq != null)
            {
                hq.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}