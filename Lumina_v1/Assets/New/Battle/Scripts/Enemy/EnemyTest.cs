using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    public enum AttackType { Ranged, Melee }

    [Header("攻击模式")]
    public AttackType attackType = AttackType.Ranged;

    [Header("属性")]
    public int maxHealth = 20;
    public int damage = 2;
    public float attackRange = 6f;
    public float attackCooldown = 3f;

    [Header("移动")]
    public float moveSpeed = 2f;
    private Vector3 destination = new Vector3(-1.25f, -2f, 10f);

    [Header("远程攻击 (仅 Ranged)")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("近战攻击 (仅 Melee)")]
    [Tooltip("生成在塔心的近战子弹 Prefab（挂 MeleeBullet.cs）")]
    public GameObject meleeBulletPrefab;
    [Tooltip("近战子弹存活时长")]
    public float meleeBulletDuration = 0.2f;

    [Header("其它")]
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

        // 优先检测范围内的塔
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
            // 进入攻击流程
            HandleAttack(towerInRange);
        }
        else
        {
            // 范围内无塔，则继续移动到 destination
            transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
        }
    }

    private void HandleAttack(Transform tower)
    {
        // 触发攻击动画
        animator.SetTrigger("Attack");

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
            return;
        }

        // 真正执行攻击
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
        // 禁用碰撞器，防止死后还能被攻击
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

    // 在 Scene 视图中可视化检测半径（可选）
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
