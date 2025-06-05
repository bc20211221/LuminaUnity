using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    public float destroyDelay = 1.5f; // 爆炸动画播放时间
    public float hitDistance = 2f;  // 命中敌人的距离阈值
    
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

        // 如果目标无效或已被销毁，寻找新目标
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            animator.SetTrigger("Hit");
            Destroy(gameObject, destroyDelay);
        }

        if (target != null)
        {

            Vector2 direction = (target.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);

            // 仅当投掷物非常接近目标时才尝试攻击
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
            // 如果没有找到任何目标，延迟后销毁自己
            Destroy(gameObject, destroyDelay);
        }
    }


    // 统一处理命中逻辑
    private void HitTarget(IDamageable damageable)
    {
        if (damageable == null || hasHit) return;

        // 触发命中动画
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        // 造成伤害
        damageable.TakeDamage(damage);
        hasHit = true;

        // 停止移动
        speed = 0f;

        // 延迟销毁
        Destroy(gameObject, destroyDelay);
    }
}

// 创建一个接口让Enemy和Boss都实现
public interface IDamageable
{
    void TakeDamage(int damage);
}