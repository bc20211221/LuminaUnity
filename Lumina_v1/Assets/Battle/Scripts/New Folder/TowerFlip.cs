using UnityEngine;

public class TowerFlip : MonoBehaviour
{
    public Transform targetEnemy; // ��ǰ����Ŀ��
    public Transform characterModel; // ��ɫģ�ͣ���Ҫת��Ĳ���

    private Quaternion originalRotation; // ԭʼ����

    void Start()
    {
        // �洢ԭʼ����
        if (characterModel != null)
        {
            originalRotation = characterModel.localRotation;
        }
    }

    void Update()
    {
        // ���û��ָ����ɫģ�ͻ�Ŀ����ˣ���ִ��ת���߼�
        if (characterModel == null || targetEnemy == null)
        {
            return;
        }

        // ����Ŀ����������Tower��λ��
        float targetPositionX = targetEnemy.position.x - transform.position.x;

        // ����Ŀ��λ�þ�������
        if (targetPositionX < 0)
        {
            // ��������ߣ�ת�����
            characterModel.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            // �������ұߣ�ת���ұߣ��ָ�ԭʼ����
            characterModel.localRotation = originalRotation;
        }
    }

    // ���õ�ǰ����Ŀ��ķ���
    public void SetTargetEnemy(Transform enemyTransform)
    {
        targetEnemy = enemyTransform;
    }
}