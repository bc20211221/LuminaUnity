using UnityEngine;

public class Tower_1 : MonoBehaviour
{
    public int maxHealth = 10;
    public int damage = 5;
    public float attackRange = 2f; // ���̹�����Χ�����ʺϽ�ս
    public float attackCooldown = 2f;
    public float meleeRadius = 1f; // ��ս�����뾶
    public GameObject meleeEffectPrefab; // ��ս����Ч��Ԥ����

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
        // ������ս����Ч��
        if (meleeEffectPrefab != null)
        {
            GameObject effect = Instantiate(meleeEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 0.5f); // ������ʾ������Ч��
        }

        // ��ⷶΧ�ڵ����е��˲�����˺�
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, meleeRadius);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                // ���������TakeDamage����
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

    // ���ӻ���ս������Χ�����ڱ༭���пɼ���
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, meleeRadius);
    }
}