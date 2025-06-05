using UnityEngine;
using System.Collections.Generic;

public class HealingTowerOnAttack : MonoBehaviour
{
    public float healingRange = 5f; // ��Ѫ��Χ
    public int healingAmount = 10; // ÿ�λ�Ѫ����
    public LayerMask towerLayer; // �ѷ�Tower���ڵ�Layer
    public Animator animator; // ����Animator���

    private void Start()
    {
        // ȷ��Animator����Ѹ�ֵ
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("δ�ҵ�Animator��������ֶ���ֵ��");
            }
        }
    }

    private void OnEnable()
    {
        // ����Animator�Ĺ��������¼�
        if (animator != null)
        {
            animator.GetBehaviour<AttackAnimationEvent>().OnAttack += HealNearbyTowers;
        }
    }

    private void OnDisable()
    {
        // ȡ������Animator�Ĺ��������¼�
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
        // ��Scene��ͼ�л��ƻ�Ѫ��Χ�Ŀ��ӻ�
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, healingRange);
    }
}

// �Զ����Animator�¼��ű�
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