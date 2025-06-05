using System.ComponentModel;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 20;
    public int damage = 2;
    public float moveSpeed = 2f;
    public float attackRange = 1f;
    public float attackCooldown = 1f;

    private int currentHealth;
    private float attackTimer;
    private Transform target;
    private HealthBar healthBar;

    public System.Action onDeath;
    private bool isAttacking = false;
    [Header("Animation Parameters")]
    public Animator animator;
    public bool isDie = false;

    [Header("Damage Number")]
    [SerializeField] private Color damageColor = Color.white;
    public DamageNumberController damageNumberController;

    void Start()
    {
        currentHealth = maxHealth;
        FindNearestTarget();

        healthBar = GetComponentInChildren<HealthBar>();
        healthBar.SetMaxHealth(maxHealth);

        animator = GetComponent<Animator>();
        gameObject.AddComponent<CircleCollider2D>();
        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
    }

    void Update()
    {
        if (target == null)
        {
            FindNearestTarget();
            if (target == null) return;
        }

        if (!isAttacking)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }

        attackTimer += Time.deltaTime;
        if (Vector2.Distance(transform.position, target.position) <= attackRange && attackTimer >= attackCooldown)
        {
            Attack();
            attackTimer = 0f;
        }
    }

    void FindNearestTarget()
    {
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

        if (nearestTower != null)
        {
            target = nearestTower;
        }
        else
        {
            GameObject hq = GameObject.FindGameObjectWithTag("Headquarters");
            if (hq != null) target = hq.transform;
        }
    }

    void Attack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        var tower = target.GetComponent<Tower>();
        if (tower != null)
        {
            tower.TakeDamage(damage);
            return;
        }

        var hq = target.GetComponent<Headquarters>();
        if (hq != null) hq.TakeDamage(damage);
    }

    public void TakeDamage(int amount)
    {
        Debug.Log($"敌人受到{amount}点伤害");
        currentHealth -= amount;
        healthBar.SetHealth(currentHealth);
        ShowDamageNumber(amount);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void ShowDamageNumber(int damageAmount)
    {
        if (DamageNumberController.Instance == null)
        {
            Debug.LogError("DamageNumberController实例未初始化！");
            return;
        }
        // 直接通过单例调用DamageNumberController
        if (DamageNumberController.Instance != null)
        {
            Vector3 position = transform.position +Vector3.up * 0.1f+Vector3.right * 0.5f;
            GameObject damageNumberGO = DamageNumberController.Instance.SpawnDamageNumber(position, damageAmount);
            DamageNumber damageNumber = damageNumberGO.GetComponent<DamageNumber>();
            if (damageNumber != null)
            {
                //damageNumber.Initialize(damageAmount, position); // 传递敌人的位置作为 enemyPosition 参数
            }
        }
    }

    void Die()
    {
        if (BossSpawner.Instance != null)
        {
            BossSpawner.Instance.EnemyDefeated();
        }

        isDie = true;
        isAttacking = true;
        BattleScoreBoard.Instance.AddScore(20);

        if (animator != null)
        {
            animator.SetBool("IsDie", true);
        }

        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
        onDeath?.Invoke();

        float destroyDelay = 1.5f;
        Destroy(gameObject, destroyDelay);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Headquarters"))
        {
            Debug.Log("u");
            target = other.transform;
            var hq = other.GetComponent<Headquarters>();
            if (hq != null)
            {
                hq.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}