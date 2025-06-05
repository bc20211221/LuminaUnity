using UnityEngine;

public class MeleeBullet : MonoBehaviour
{
    private int damage;
    private float range;
    [HideInInspector] public float duration = 0.2f;

    /// <summary>
    /// �����˺�ֵ
    /// </summary>
    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    /// <summary>
    /// ������Ч��Χ��EN.attackRange��
    /// </summary>
    public void SetRange(float r)
    {
        range = r;
    }

    void Start()
    {
        // ������һ�η�Χ�˺�
        ApplyAoE();

        // ������Ч������
        Destroy(gameObject, duration);
    }

    private void ApplyAoE()
    {
        // �ҵ���Χ�����������������ɱ��˺���Ŀ�꣩
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

    // ��ѡ���ڱ༭���п��ӻ���Χ
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
