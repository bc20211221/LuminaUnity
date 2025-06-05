using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    public int maxHealth = 20;
    public int damage = 2;
    public float moveSpeed = 2f;
    public float attackRange = 5f; // ���ӹ�����Χ
    public float attackCooldown = 1f;
    public Projectile projectilePrefab; // �ӵ�Ԥ����
    public Transform firePoint; // �ӵ������

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

        // ��ʼ��Ѫ��
        healthBar = GetComponentInChildren<HealthBar>();
        healthBar.SetMaxHealth(maxHealth);

        animator = GetComponent<Animator>();
        gameObject.AddComponent<CircleCollider2D>();
        CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
    }

    void Update()
    {
        // �����ǰĿ�겻���ڣ�Ѱ����Ŀ��
        if (target == null)
        {
            FindNearestTarget();
            if (target == null) return; // �����Ȼû��Ŀ�꣬�˳�
        }

        // �ƶ��߼� - �����ڹ�����Χ��
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        if (distanceToTarget > attackRange && !isAttacking)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
        else if (distanceToTarget < attackRange * 0.8f)
        {
            // ���̫��������һ��
            Vector2 direction = (transform.position - target.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime * 0.5f);
        }

        // ����Ŀ��
        if (target != null)
        {
            Vector2 direction = target.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        // �����߼�
        attackTimer += Time.deltaTime;
        if (distanceToTarget <= attackRange && attackTimer >= attackCooldown)
        {
            Attack();
            attackTimer = 0f;
        }
    }

    void FindNearestTarget()
    {
        // ����Ѱ����
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

        // ����ҵ�������ΪĿ��
        if (nearestTower != null)
        {
            target = nearestTower;
        }
        else
        {
            // û�����򹥻��ܲ�
            GameObject hq = GameObject.FindGameObjectWithTag("Headquarters");
            if (hq != null) target = hq.transform;
        }
    }

    void Attack()
    {
        // ������������
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // �����ӵ�
        if (projectilePrefab != null && target != null)
        {
            Projectile projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            //projectile.damage = damage;
           // projectile.target = target;
            projectile.speed = 5f; // �����ӵ��ٶ�
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
        // ֪ͨBoss���������˱�����
        if (BossSpawner.Instance != null)
        {
            BossSpawner.Instance.EnemyDefeated();
        }

        isDie = true;
        isAttacking = true; // ֹͣ�ƶ�
        BattleScoreBoard.Instance.AddScore(20);

        // ������������
        if (animator != null)
        {
            animator.SetBool("IsDie", true);
        }

        // ������ײ������ֹ�����ܱ�����
        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
        onDeath?.Invoke();
        // �ӳ����ٶ����������������㹻ʱ�䲥��
        float destroyDelay = 1.5f; // ������������������ȵ���
        Destroy(gameObject, destroyDelay);
    }

    // ��ײ���
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Headquarters"))
        {
            target = other.transform; // ȷ��Ŀ���� Headquarters
            var hq = other.GetComponent<Headquarters>();
            if (hq != null)
            {
                hq.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}