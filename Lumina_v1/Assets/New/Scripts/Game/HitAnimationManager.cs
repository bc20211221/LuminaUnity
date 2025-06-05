using UnityEngine;
using System.Collections;

public class HitAnimationManager : MonoBehaviour
{
    [Header("�����������")]
    public GameObject[] laneHitAnimations; // ʹ��GameObject���飬��Ӧ�ĸ�����Ķ���Ԥ����
    public float animationDuration = 0.2f; // ��������ʱ�䣨����ʵ�ʶ���ʱ��ƥ�䣩

    private void Start()
    {
        // ��ʼ���������ж���
        for (int i = 0; i < laneHitAnimations.Length; i++)
        {
            if (laneHitAnimations[i])
                laneHitAnimations[i].SetActive(false); // ֱ�Ӳ���GameObject�ļ���״̬
        }
    }

    /// <summary>
    /// ����ָ������Ĵ������
    /// </summary>
    /// <param name="laneIndex">���������0-3��</param>
    public void PlayAnimation(int laneIndex)
    {
        if (laneIndex < 0 || laneIndex >= laneHitAnimations.Length)
        {
            Debug.LogError($"��Ч������� {laneIndex}������Ϊ0-{laneHitAnimations.Length - 1}��");
            return;
        }

        GameObject animationGO = laneHitAnimations[laneIndex];
        if (!animationGO)
        {
            Debug.LogError($"��� {laneIndex} �Ķ���GameObjectδ��ֵ��");
            return;
        }

        // ��ʾ��������ʼЭ�̿�������
        animationGO.SetActive(false);
        animationGO.SetActive(true);
        StartCoroutine(HideAnimationAfterDelay(laneIndex));
    }

    /// <summary>
    /// �ӳٺ�����ָ������Ķ���
    /// </summary>
    private IEnumerator HideAnimationAfterDelay(int laneIndex)
    {
        yield return new WaitForSeconds(animationDuration);

        if (laneIndex >= 0 && laneIndex < laneHitAnimations.Length)
        {
            laneHitAnimations[laneIndex].SetActive(false); // ���ض���GameObject
        }
    }
}