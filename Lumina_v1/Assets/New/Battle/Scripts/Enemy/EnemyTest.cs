using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest : MonoBehaviour
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
    private Vector3 destination = new Vector3(-1.25f, -2f, 10f);

    [Header("Զ�̹��� (�� Ranged)")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("��ս���� (�� Melee)")]
    [Tooltip("���������ĵĽ�ս�ӵ� Prefab���� MeleeBullet.cs��")]
    public GameObject meleeBulletPrefab;
    [Tooltip("��ս�ӵ����ʱ��")]
    public float meleeBulletDuration = 0.2f;

    [Header("����")]
    public Animator animator;
    public System.Action onDeath;

    private int currentHealth;
    private HealthBar healthBar;
    private float attackTimer = 0f;
    private bool isDie = false;

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

        // ���ȼ�ⷶΧ�ڵ���
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
            // ���빥������
            HandleAttack(towerInRange);
        }
        else
        {
            // ��Χ��������������ƶ��� destination
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
        }
    }

    private void HandleAttack(Transform tower)
    {
        // ������������
        animator.SetTrigger("Attack");

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
            return;
        }

        // ����ִ�й���
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
        if (isDie) return;

        currentHealth -= damageAmount;
        if (healthBar != null) healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        // ������ײ������ֹ�����ܱ�����
        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
        isDie = true;
        animator.SetBool("IsDie", true);

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        BossSpawner.Instance?.EnemyDefeated();
        onDeath?.Invoke();
        BattleScoreBoard.Instance.AddScore(20);

        Destroy(gameObject, 1.5f);
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

    // �� Scene ��ͼ�п��ӻ����뾶����ѡ��
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
