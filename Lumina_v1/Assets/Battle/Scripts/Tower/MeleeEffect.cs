using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class MeleeEffect : MonoBehaviour
{
    [HideInInspector] public int damage = 1;
    [HideInInspector] public float duration = 0.5f;

    private CircleCollider2D circleCol;

    void Start()
    {
        // 确保是 Trigger
        circleCol = GetComponent<CircleCollider2D>();
        circleCol.isTrigger = true;

        // 一次性范围伤害
        DealAoeDamage();

        // 播放完特效后自动销毁
        Destroy(gameObject, duration);
    }

    private void DealAoeDamage()
    {
        // 以自身位置和 Collider 半径为范围，扫描所有 Collider
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, circleCol.radius);
        foreach (var hit in hits)
        {
            // 跳过自己
            if (hit.gameObject == gameObject) continue;

            // 优先 EN
            var en = hit.GetComponent<EN>();
            if (en != null)
            {
                en.TakeDamage(damage);
            }

            // 再试 Boss
            var boss = hit.GetComponent<Boss>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
            }
        }
    }

    // 可选：在编辑器里可视化攻击范围
    void OnDrawGizmosSelected()
    {
        var col = GetComponent<CircleCollider2D>();
        if (col != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, col.radius);
        }
    }
}
