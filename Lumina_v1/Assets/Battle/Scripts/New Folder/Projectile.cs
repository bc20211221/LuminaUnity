using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    public float destroyDelay = 1.5f; // ��ը��������ʱ��
    public float hitDistance = 2f;  // ���е��˵ľ�����ֵ
    
    private Transform target;
    private int damage;
    private bool hasHit = false;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    void Update()
    {
        if (hasHit)
        {
            return;
        }

        // ���Ŀ����Ч���ѱ����٣�Ѱ����Ŀ��
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            animator.SetTrigger("Hit");
            Destroy(gameObject, destroyDelay);
        }

        if (target != null)
        {

            Vector2 direction = (target.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);

            // ����Ͷ����ǳ��ӽ�Ŀ��ʱ�ų��Թ���
            if (Vector2.Distance(transform.position, target.position) < hitDistance)
            {
                Debug.Log("Close");
                IDamageable damageable = target.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    HitTarget(damageable);
                }
            }
        }
        else
        {
            // ���û���ҵ��κ�Ŀ�꣬�ӳٺ������Լ�
            Destroy(gameObject, destroyDelay);
        }
    }


    // ͳһ���������߼�
    private void HitTarget(IDamageable damageable)
    {
        if (damageable == null || hasHit) return;

        // �������ж���
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        // ����˺�
        damageable.TakeDamage(damage);
        hasHit = true;

        // ֹͣ�ƶ�
        speed = 0f;

        // �ӳ�����
        Destroy(gameObject, destroyDelay);
    }
}

// ����һ���ӿ���Enemy��Boss��ʵ��
public interface IDamageable
{
    void TakeDamage(int damage);
}