using UnityEngine;

public class BattleArea : MonoBehaviour
{
    public float myAreaRadius = 5f;        // �ҷ���Բ����뾶
    public float enemyAreaRadius = 8f;     // �з���Բ����뾶
    public float myRotationAngle = 90f;    // �ҷ���Բ��ת�Ƕ�
    public float enemyRotationAngle = 90f;// �з���Բ��ת�Ƕ�
    public int arcSections = 4;            // �ֶ�����

    void OnDrawGizmos()
    {
        // �����ҷ���Բ����
        Gizmos.color = Color.blue;
        DrawSemiCircle(transform.position, myAreaRadius, 180f, arcSections, myRotationAngle);

        // ���Ƶз���Բ����
        Gizmos.color = Color.red;
        DrawSemiCircle(transform.position, enemyAreaRadius, 180f, arcSections, enemyRotationAngle);
    }

    void DrawSemiCircle(Vector3 center, float radius, float angle, int sections, float offsetAngle)
    {
        float sectionAngle = angle / sections;
        for (int i = 0; i < sections; i++)
        {
            float startAngle = -angle / 2 + i * sectionAngle + offsetAngle;
            float endAngle = startAngle + sectionAngle;
            DrawArc(center, radius, startAngle, endAngle);
        }
    }

    void DrawArc(Vector3 center, float radius, float startAngle, float endAngle)
    {
        int segments = 10;
        Vector3 prevPos = center + Quaternion.Euler(0, 0, startAngle) * Vector3.right * radius;
        for (int i = 1; i <= segments; i++)
        {
            float angle = Mathf.Lerp(startAngle, endAngle, i / (float)segments);
            Vector3 newPos = center + Quaternion.Euler(0, 0, angle) * Vector3.right * radius;
            Gizmos.DrawLine(prevPos, newPos);
            prevPos = newPos;
        }

        // ��ѡ����������ָ�����е�
        Gizmos.DrawLine(center, center + Quaternion.Euler(0, 0, (startAngle + endAngle) / 2) * Vector3.right * radius);
    }
}
