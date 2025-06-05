using UnityEngine;

public class MeleeBullet : MonoBehaviour
{
    private int damage;
    private float range;
    [HideInInspector] public float duration = 0.2f;

    /// <summary>
    /// 设置伤害值
    /// </summary>
    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    /// <summary>
    /// 设置生效范围（EN.attackRange）
    /// </summary>
    public void SetRange(float r)
    {
        range = r;
    }

    void Start()
    {
        // 立即做一次范围伤害
        ApplyAoE();

        // 持续特效后销毁
        Destroy(gameObject, duration);
    }

    private void ApplyAoE()
    {
        // 找到范围内所有塔（或其它可被伤害的目标）
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Tower"))
            {
                var tower = hit.GetComponent<IDamageable>();
                if (tower != null)
                    tower.TakeDamage(damage);
            }
        }
    }

    // 可选：在编辑器中可视化范围
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
