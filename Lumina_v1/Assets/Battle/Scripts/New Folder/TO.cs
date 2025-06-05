using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TO : MonoBehaviour, IDamageable
{
    public enum AttackType { Ranged, Melee }

    [Header("通用设置")]
    public AttackType attackType = AttackType.Ranged;
    public int maxHealth = 20;
    //生物造成的伤害值
    [SerializeField] public int towerID = 0;
    [SerializeField] private int damage = 2;
    //public int damage = 2;
    public float attackRange = 5f;
    public Animator animator;

    [Header("远程攻击")]
    public GameObject bulletPrefab;

    [Header("近战攻击")]
    [Tooltip("需要一个带有 Trigger Collider2D 半径=attackRange 的近战攻击特效预制体")]
    public GameObject meleeEffectPrefab;
    [Tooltip("近战攻击特效持续时间，控制特效显示时长")]
    public float meleeDuration = 0.5f;

    public float healingRange = 5f; 
    public int healingAmount = 10; 
    public LayerMask towerLayer; 

    private int currentHealth;
    private Transform target;
    private HealthBar healthBar;
    private int towerType; 
    private TowerFlip orientationController;

    void Start()
    {
        currentHealth = maxHealth;
        animator.SetTrigger("Appear");

        var hb = GetComponentInChildren<HealthBar>();
        if (hb != null)
        {
            healthBar = hb;
            healthBar.SetMaxHealth(maxHealth);
        }
        orientationController = GetComponent<TowerFlip>();
        //初始化同步一次伤害值
        SetTowerDamage();
    }

    //动态获取当前的伤害值数组
    private int[] GetCurrentTowerDamages() =>
        new[] {BuffAttribute.OneAttack,BuffAttribute.TwoAttack,BuffAttribute.ThreeAttack,BuffAttribute.FourAttack};
    //根据塔ID设置伤害值
    private void SetTowerDamage()
    {
        var currentDamages = GetCurrentTowerDamages();
        if(towerID >=0 && towerID < currentDamages.Length)
        {
            damage = currentDamages[towerID];
        }
        else
        {
            damage = 2;
        }
    }
    //时刻更新伤害值
    void Update()
    {
        SetTowerDamage();
    }
    public void ForceAttack()
    {
        if (IsDead()) return;

        GameObject nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            PerformAttack(nearestEnemy.transform);
        }
    }

    private void PerformAttack(Transform target)
    {
        float distance = Vector2.Distance(transform.position, target.position);

        animator.SetTrigger("Attack");

        HealNearbyTowers();

        if (attackType == AttackType.Ranged)
            FireBullet(target);
        else
        {
            if (distance > attackRange)
            {
                return; 
            }
            StartCoroutine(DoMelee());
        }
        if (orientationController != null)
        {
            orientationController.SetTargetEnemy(target);
        }
    }

    private void FireBullet(Transform target)
    {
        if (bulletPrefab == null || target == null) return;

        var go = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        var bs = go.GetComponent<Bullet>();
        if (bs != null)
        {
            bs.SetDamage(damage);
            bs.SetTarget(target);
        }
    }

    private IEnumerator DoMelee()
    {
        if (meleeEffectPrefab == null)
        {
            Debug.LogWarning("未指定 meleeEffectPrefab！");
            yield break;
        }

        var fx = Instantiate(meleeEffectPrefab, transform.position, Quaternion.identity);
        var me = fx.GetComponent<MeleeEffect>();
        if (me != null)
        {
            me.damage = damage;
            me.duration = meleeDuration;
        }
        else
        {
            Debug.LogWarning("meleeEffectPrefab 缺少 MeleeEffect 组件");
        }

        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D enemyCollider in enemiesInRange)
        {
            IDamageable damageableEnemy = enemyCollider.GetComponent<IDamageable>();
            if (damageableEnemy != null && enemyCollider.CompareTag("Enemy") || enemyCollider.CompareTag("Boss"))
            {
                damageableEnemy.TakeDamage(damage);
            }
        }

        yield return new WaitForSeconds(meleeDuration);
        Destroy(fx);
    }

    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss");
        List<GameObject> allTargets = new List<GameObject>();
        allTargets.AddRange(enemies);
        allTargets.AddRange(bosses);

        GameObject nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject target in allTargets)
        {
            if (target == null) continue;

            var enemyComponent = target.GetComponent<EN>();
            var bossComponent = target.GetComponent<Boss>();
            bool isAlive = (enemyComponent != null && !enemyComponent.IsDie) || (bossComponent != null);

            if (isAlive)
            {
                float distance = Vector2.Distance(transform.position, target.transform.position);
                if (distance < minDistance && distance <= attackRange)
                {
                    minDistance = distance;
                    nearest = target;
                }
            }
        }

        return nearest;
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        ShowDamageNumber(dmg, Color.red); 
        if (healthBar != null)
            healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        ShowDamageNumber(healAmount, Color.green); 
        if (healthBar != null)
            healthBar.SetHealth(currentHealth);
    }
    //保存Type索引
    private TowerPlacer towerPlacer;
    public int TowerTypeIndex { get; set; }
    public void SetTowerPlacer(TowerPlacer placer)
    {
        towerPlacer = placer;
    }

    private void Die()
    {
        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
        if (towerPlacer != null)
        {
            towerPlacer.OnTowerDestroyed(gameObject);
        }
        animator.SetBool("IsDie", true);
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        Destroy(gameObject, 1.5f);
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    public void SetTowerType(int type)
    {
        towerType = type;
    }

    public int TowerType
    {
        get { return towerType; }
    }

    private void ShowDamageNumber(int amount, Color color)
    {
        if (DamageNumberController.Instance == null)
        {
            Debug.LogError("DamageNumberController 实例未初始化");
            return;
        }
        if (DamageNumberController.Instance != null)
        {
            Vector3 position = transform.position + Vector3.up * 0.1f + Vector3.right * 0.5f;
            GameObject damageNumberGO = DamageNumberController.Instance.SpawnDamageNumber(position, amount);
            DamageNumber damageNumber = damageNumberGO.GetComponent<DamageNumber>();
            if (damageNumber != null)
            {
                damageNumber.Initialize(amount, position, color); 
            }
        }
    }

    private void HealNearbyTowers()
    {
        Collider2D[] towersInRange = Physics2D.OverlapCircleAll(transform.position, healingRange, towerLayer);
        foreach (Collider2D towerCollider in towersInRange)
        {
            TO tower = towerCollider.GetComponent<TO>();
            if (tower != null && !tower.IsDead())
            {
                tower.Heal(healingAmount);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, healingRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}