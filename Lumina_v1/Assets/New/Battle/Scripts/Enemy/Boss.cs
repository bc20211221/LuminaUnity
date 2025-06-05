using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.Process;

public class Boss : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;  // Bossͨ��ӵ�нϸߵ�Ѫ��
    public int damage = 5;       // Bossͨ��ӵ�нϸߵ��˺�
    public float attackRange = 3f; // Boss����ʱ�Ĺ�����Χ
    public float attackCooldown = 2f;
    public float detectionRange = 10f; // Boss�ļ�ⷶΧ
    [SerializeField] private GameObject attackEffectPrefab; // ������ЧԤ����
    [SerializeField] private float attackEffectDuration = 0.5f;
    [SerializeField] private Color damageColor = Color.white; // �˺�������ɫ
    public DamageNumberController damageNumberController; // �˺����ֿ�����

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

        // ��ʼ��Ѫ��
        healthBar = GetComponentInChildren<HealthBar>();
        healthBar.SetMaxHealth(BuffAttribute.BossBlood);
    }

    void Update()
    {
        // �����߼�
        attackTimer += Time.deltaTime;

        // Ѱ�������Ŀ��
        FindNearestTarget();

        // ���û��Ŀ�ֹ꣬ͣ�����߼�
        if (target == null)
        {
            return;
        }

        // �ж�Ŀ���Ƿ��ڹ�����Χ��
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

        // ��ȡ������
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower")
            .Where(t => t != null).ToArray(); // �Ƴ��Ѿ������ٵĶ���

        if (towers.Length == 0)
        {
            yield break; // ���û��Ŀ��ֱ���˳�Э��
        }

        // ��������������ڵ���3�������ѡ��3��������ѡ������
        List<Transform> targets = towers.Length >= 3
            ? towers.OrderBy(t => Random.value).Take(3)
                .Where(t => t != null) // �ٴι���
                .Select(t => t.transform).ToList()
            : towers.Where(t => t != null)
                .Select(t => t.transform).ToList();

        foreach (Transform towerTransform in targets)
        {
            // ���Ŀ���Ƿ���Ȼ����
            if (towerTransform == null || towerTransform.gameObject == null)
            {
                continue; // �����Ѿ������ٵ�Ŀ��
            }

            // ���Ź�����Ч
            if (attackEffectPrefab != null)
            {
                GameObject effect = Instantiate(attackEffectPrefab, towerTransform.position, Quaternion.identity);
                yield return new WaitForSeconds(attackEffectDuration);
                Destroy(effect);
            }

            // ��Ŀ������˺�
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

        ShowDamageNumber(amount, damageColor); // ��ʾ�˺�����

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
        float destroyDelay = 1.5f; // �ӳ�һ��ʱ�������ٶ���
        Destroy(gameObject, destroyDelay);
        ComboPresenter.Instance.AddEnemyKill();
    }

    private void ShowDamageNumber(int damageAmount, Color color)
    {
        if (DamageNumberController.Instance == null)
        {
            Debug.LogError("DamageNumberController ʵ��δ��ʼ��");
            return;
        }
        // ֱ��ͨ���������� DamageNumberController
        if (DamageNumberController.Instance != null)
        {
            Vector3 position = transform.position + Vector3.up * 0.1f + Vector3.right * 0.5f;
            GameObject damageNumberGO = DamageNumberController.Instance.SpawnDamageNumber(position, damageAmount);
            DamageNumber damageNumber = damageNumberGO.GetComponent<DamageNumber>();
            if (damageNumber != null)
            {
                damageNumber.Initialize(damageAmount, position, color); // ���ݴ������ɫ�����˺�������ɫ
            }
        }
    }
}