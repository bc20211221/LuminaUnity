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
    private int towerType; // ��������������

    // ������ֻ�����ԣ��ж����Ƿ�������
    public bool IsDead => isDie;
    public int TowerType => towerType;

    void Start()
    {
        isDie = false;
        currentHealth = maxHealth;

        // ��ȡ�Ӷ����е�Ѫ�����
        HealthBar healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }

        // ��ʼ����������ײ��
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

    // �Ƴ�Update�е��Զ������߼�

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

    // ǿ�ƹ������������ⲿ������
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

        // ���ɵ����ﲢ����Ŀ����˺�
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

        // ����Ѫ��
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

    // ����TowerPlacer����
    public void SetTowerPlacer(TowerPlacer placer)
    {
        towerPlacer = placer;
    }

    // ������������������
    public void SetTowerType(int type)
    {
        towerType = type;
    }

    void Die()
    {
        if (isDie) return;

        isDie = true;

        // ֪ͨ������
        if (towerPlacer != null)
        {
            towerPlacer.OnTowerDestroyed(gameObject);
        }

        // �����Ӿ�Ч��������
        if (animator != null)
        {
            animator.SetBool("IsDie", true);
        }

        // ������ײ��
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // �ӳ����ٶ���
        Destroy(gameObject, 1.5f);
    }
}