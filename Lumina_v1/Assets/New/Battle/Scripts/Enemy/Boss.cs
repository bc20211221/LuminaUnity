using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.Process;

public class Boss : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;  // Boss通常拥有较高的血量
    public int damage = 5;       // Boss通常拥有较高的伤害
    public float attackRange = 3f; // Boss攻击时的攻击范围
    public float attackCooldown = 2f;
    public float detectionRange = 10f; // Boss的检测范围
    [SerializeField] private GameObject attackEffectPrefab; // 攻击特效预制体
    [SerializeField] private float attackEffectDuration = 0.5f;
    [SerializeField] private Color damageColor = Color.white; // 伤害数字颜色
    public DamageNumberController damageNumberController; // 伤害数字控制器

    private int currentHealth;
    private float attackTimer;
    private Transform target;
    private HealthBar healthBar;
    private Animator animator;

    public System.Action onDeath;

    void Start()
    {
        animator = GetComponent<Animator>();
        gameObject.AddComponent<CircleCollider2D>();
        currentHealth = BuffAttribute.BossBlood;

        // 初始化血量
        healthBar = GetComponentInChildren<HealthBar>();
        healthBar.SetMaxHealth(BuffAttribute.BossBlood);
    }

    void Update()
    {
        // 攻击逻辑
        attackTimer += Time.deltaTime;

        // 寻找最近的目标
        FindNearestTarget();

        // 如果没有目标，停止攻击逻辑
        if (target == null)
        {
            return;
        }

        // 判断目标是否在攻击范围内
        if (Vector2.Distance(transform.position, target.position) <= attackRange)
        {
            if (attackTimer >= attackCooldown)
            {
                StartCoroutine(MultiTargetAttack());
                attackTimer = 0f;
            }
        }
    }

    private void FindNearestTarget()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        Transform nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject tower in towers)
        {
            if (tower == null) continue;

            float distance = Vector2.Distance(transform.position, tower.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = tower.transform;
            }
        }

        target = nearest;
    }

    IEnumerator MultiTargetAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // 获取所有塔
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower")
            .Where(t => t != null).ToArray(); // 移除已经被销毁的对象

        if (towers.Length == 0)
        {
            yield break; // 如果没有目标直接退出协程
        }

        // 如果塔的数量大于等于3个，随机选择3个，否则选择所有
        List<Transform> targets = towers.Length >= 3
            ? towers.OrderBy(t => Random.value).Take(3)
                .Where(t => t != null) // 再次过滤
                .Select(t => t.transform).ToList()
            : towers.Where(t => t != null)
                .Select(t => t.transform).ToList();

        foreach (Transform towerTransform in targets)
        {
            // 检查目标是否仍然存在
            if (towerTransform == null || towerTransform.gameObject == null)
            {
                continue; // 跳过已经被销毁的目标
            }

            // 播放攻击特效
            if (attackEffectPrefab != null)
            {
                GameObject effect = Instantiate(attackEffectPrefab, towerTransform.position, Quaternion.identity);
                yield return new WaitForSeconds(attackEffectDuration);
                Destroy(effect);
            }

            // 对目标造成伤害
            IDamageable damageableObject = towerTransform.GetComponent<IDamageable>();
            if (damageableObject != null)
            {
                damageableObject.TakeDamage(damage);
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

        ShowDamageNumber(amount, damageColor); // 显示伤害数字

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        animator.SetTrigger("Die");
        onDeath?.Invoke();
        BattleScoreBoard.Instance.AddScore(1000);
        float destroyDelay = 1.5f; // 延迟一段时间再销毁对象
        Destroy(gameObject, destroyDelay);
        ComboPresenter.Instance.AddEnemyKill();
    }

    private void ShowDamageNumber(int damageAmount, Color color)
    {
        if (DamageNumberController.Instance == null)
        {
            Debug.LogError("DamageNumberController 实例未初始化");
            return;
        }
        // 直接通过单例调用 DamageNumberController
        if (DamageNumberController.Instance != null)
        {
            Vector3 position = transform.position + Vector3.up * 0.1f + Vector3.right * 0.5f;
            GameObject damageNumberGO = DamageNumberController.Instance.SpawnDamageNumber(position, damageAmount);
            DamageNumber damageNumber = damageNumberGO.GetComponent<DamageNumber>();
            if (damageNumber != null)
            {
                damageNumber.Initialize(damageAmount, position, color); // 根据传入的颜色设置伤害数字颜色
            }
        }
    }
}