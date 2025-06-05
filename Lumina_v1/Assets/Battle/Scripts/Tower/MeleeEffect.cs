using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class MeleeEffect : MonoBehaviour
{
    [HideInInspector] public int damage = 1;
    [HideInInspector] public float duration = 0.5f;

    private CircleCollider2D circleCol;

    void Start()
    {
        // ȷ���� Trigger
        circleCol = GetComponent<CircleCollider2D>();
        circleCol.isTrigger = true;

        // һ���Է�Χ�˺�
        DealAoeDamage();

        // ��������Ч���Զ�����
        Destroy(gameObject, duration);
    }

    private void DealAoeDamage()
    {
        // ������λ�ú� Collider �뾶Ϊ��Χ��ɨ������ Collider
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, circleCol.radius);
        foreach (var hit in hits)
        {
            // �����Լ�
            if (hit.gameObject == gameObject) continue;

            // ���� EN
            var en = hit.GetComponent<EN>();
            if (en != null)
            {
                en.TakeDamage(damage);
            }

            // ���� Boss
            var boss = hit.GetComponent<Boss>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
            }
        }
    }

    // ��ѡ���ڱ༭������ӻ�������Χ
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
