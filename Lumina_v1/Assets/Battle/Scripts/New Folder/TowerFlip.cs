using UnityEngine;

public class TowerFlip : MonoBehaviour
{
    public Transform targetEnemy; // 当前攻击目标
    public Transform characterModel; // 角色模型，需要转向的部分

    private Quaternion originalRotation; // 原始朝向

    void Start()
    {
        // 存储原始朝向
        if (characterModel != null)
        {
            originalRotation = characterModel.localRotation;
        }
    }

    void Update()
    {
        // 如果没有指定角色模型或目标敌人，则不执行转向逻辑
        if (characterModel == null || targetEnemy == null)
        {
            return;
        }

        // 计算目标敌人相对于Tower的位置
        float targetPositionX = targetEnemy.position.x - transform.position.x;

        // 根据目标位置决定朝向
        if (targetPositionX < 0)
        {
            // 敌人在左边，转向左边
            characterModel.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            // 敌人在右边，转向右边（恢复原始朝向）
            characterModel.localRotation = originalRotation;
        }
    }

    // 设置当前攻击目标的方法
    public void SetTargetEnemy(Transform enemyTransform)
    {
        targetEnemy = enemyTransform;
    }
}