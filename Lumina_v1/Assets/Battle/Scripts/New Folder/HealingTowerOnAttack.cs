using UnityEngine;
using System.Collections.Generic;

public class HealingTowerOnAttack : MonoBehaviour
{
    public float healingRange = 5f; // 回血范围
    public int healingAmount = 10; // 每次回血的量
    public LayerMask towerLayer; // 友方Tower所在的Layer
    public Animator animator; // 塔的Animator组件

    private void Start()
    {
        // 确保Animator组件已赋值
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("未找到Animator组件，请手动赋值。");
            }
        }
    }

    private void OnEnable()
    {
        // 订阅Animator的攻击动画事件
        if (animator != null)
        {
            animator.GetBehaviour<AttackAnimationEvent>().OnAttack += HealNearbyTowers;
        }
    }

    private void OnDisable()
    {
        // 取消订阅Animator的攻击动画事件
        if (animator != null)
        {
            animator.GetBehaviour<AttackAnimationEvent>().OnAttack -= HealNearbyTowers;
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
        // 在Scene视图中绘制回血范围的可视化
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, healingRange);
    }
}

// 自定义的Animator事件脚本
public class AttackAnimationEvent : StateMachineBehaviour
{
    public System.Action OnAttack;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Attack"))
        {
            if (OnAttack != null)
            {
                OnAttack.Invoke();
            }
        }
    }
}