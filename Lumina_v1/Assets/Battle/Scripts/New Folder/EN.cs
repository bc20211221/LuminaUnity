using UnityEngine;
using Game.Process;
using System.ComponentModel;

public class EN : MonoBehaviour
{
    public enum AttackType { Ranged, Melee }

    [Header("����ģʽ")]
    public AttackType attackType = AttackType.Ranged;

    [Header("����")]
    public int maxHealth = 20;
    public int damage = 2;
    public float attackRange = 6f;
    public float attackCooldown = 3f;

    [Header("�ƶ�")]
    public float moveSpeed = 2f;
    private Vector3 destination = new Vector3(-1.25f, -3f, 10f);

    [Header("Զ�̹��� (�� Ranged)")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("��ս���� (�� Melee)")]
    [Tooltip("��ս������Ч Prefab����Ҫ MeleeBullet.cs �ű�")]
    public GameObject meleeBulletPrefab;
    [Tooltip("��ս������Ч��ʾʱ��")]
    public float meleeBulletDuration = 0.2f;

    [Header("����")]
    public Animator animator;
    public System.Action onDeath;

    [Header("Damage Number")]
    [SerializeField] private Color damageColor = Color.white;
    public DamageNumberController damageNumberController;

    private int currentHealth;
    private HealthBar healthBar;
    private float attackTimer = 0f;
    private bool isDie = false;

    public bool IsDie
    {
        get { return isDie; }
    }

    void Start()
    {
        currentHealth = maxHealth;
        var hb = GetComponentInChildren<HealthBar>();
        if (hb != null)
        {
            healthBar = hb;
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    void Update()
    {
        if (isDie) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        Transform towerInRange = null;
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Tower"))
            {
                towerInRange = hit.transform;
                break;
            }
        }

        if (towerInRange != null)
        {
            HandleAttack(towerInRange);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
        }
    }

    private void HandleAttack(Transform tower)
    {
        animator.SetTrigger("Attack");

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
            return;
        }
        if (attackType == AttackType.Ranged)
            DoRangedAttack(tower);
        else
            DoMeleeAttack(tower);

        attackTimer = attackCooldown;
    }

    private void DoRangedAttack(Transform tower)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject b = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        var bs = b.GetComponent<Bullet>();
        if (bs != null)
        {
            bs.SetDamage(damage);
            bs.SetTarget(tower);
        }
    }

    private void DoMeleeAttack(Transform tower)
    {
        if (meleeBulletPrefab == null) return;

        GameObject mb = Instantiate(meleeBulletPrefab, tower.position, Quaternion.identity);
        var mbs = mb.GetComponent<MeleeBullet>();
        if (mbs != null)
        {
            mbs.SetDamage(damage);
            mbs.SetRange(attackRange);
            mbs.duration = meleeBulletDuration;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        //Debug.Log($"�ܵ�{damageAmount}���˺�");
        if (isDie) return;
        currentHealth -= damageAmount;
        ShowDamageNumber(damageAmount, Color.white); 
        if (healthBar != null) healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
        isDie = true;
        //��������ʱ���
        if(BuffAttribute.Thiskillbuff)
        {
            BuffAttribute.Thiskill = true;
        }

        animator.SetBool("IsDie", true);

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        BossSpawner.Instance?.EnemyDefeated();
        onDeath?.Invoke();
        BattleScoreBoard.Instance.AddScore(50);

        Destroy(gameObject, 1.5f);
        ComboPresenter.Instance.AddEnemyKill();
    }

    private void ShowDamageNumber(int damageAmount, Color color)
    {
        if (DamageNumberController.Instance == null)
        {
            Debug.LogError("DamageNumberController ʵ��δ��ʼ��");
            return;
        }
        if (DamageNumberController.Instance != null)
        {
            Vector3 position = transform.position + Vector3.up * 0.1f + Vector3.right * 0.5f;
            GameObject damageNumberGO = DamageNumberController.Instance.SpawnDamageNumber(position, damageAmount);
            DamageNumber damageNumber = damageNumberGO.GetComponent<DamageNumber>();
            if (damageNumber != null)
            {
                damageNumber.Initialize(damageAmount, position, color);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Headquarters"))
        {
            BossSpawner.Instance?.EnemyDefeated();
            onDeath?.Invoke();
            var hq = other.GetComponent<Headquarters>();
            if (hq != null) hq.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}