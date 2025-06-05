using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    private int damage;
    private Transform target;
    private bool hasHit = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        // If there's no Animator component, add one
        if (animator == null)
        {
            animator = gameObject.AddComponent<Animator>();
        }
    }

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    public void SetTarget(Transform trg)
    {
        target = trg;
    }

    void Update()
    {
        if (hasHit) return; // Don't move after hitting

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Simple collision detection
        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        if (hasHit) return; // Prevent multiple hits
        hasHit = true;

        // Trigger hit animation
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        // Apply damage
        if (target != null && target.CompareTag("Enemy"))
        {
            var enemy = target.GetComponent<EN>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            else
            {
                var boss = target.GetComponent<Boss>();
                if (boss != null)
                {
                    boss.TakeDamage(damage);
                }
            }
        }
        else if (target != null && target.CompareTag("Tower"))
        {
            Debug.Log(damage);
            target.GetComponent<TO>().TakeDamage(damage);
        }

        // Disable collider and stop movement
        var collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        // Destroy after animation (0.6f seconds)
        Destroy(gameObject, 0.6f);
    }
}