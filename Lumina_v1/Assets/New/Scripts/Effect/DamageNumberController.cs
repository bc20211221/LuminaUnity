using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class DamageNumberController : MonoBehaviour
{
    public static DamageNumberController Instance; // ȷ������ʵ��
    public GameObject damageNumberPrefab; // ʹ��GameObject���ͣ�������UI��TextMeshPro
    public Transform canvasTransform; // ȷ����ȡ��Canvas��Transform

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // ������ҪһЩ��Դ�ͷŵ��߼������������
            Destroy(gameObject);
        }
    }

    // �����˺����ֵķ���
    public GameObject SpawnDamageNumber(Vector3 worldPosition, int damageValue)
    {
        Debug.Log($"�����˺�����λ�ã�{worldPosition}"); // �������
        if (damageNumberPrefab == null)
        {
            Debug.LogError("DamageNumberPrefabδ��ֵ��");
            return null;
        }

        // ʵ����Ԥ���嵽Canvas��
        GameObject damageNumberGO = Instantiate(
            damageNumberPrefab,
            worldPosition,
            Quaternion.identity,
            canvasTransform
        );

        // ��ȡ�ı������������TextMeshPro��Unity UI Text
        TMP_Text tmpText = damageNumberGO.GetComponent<TMP_Text>();
        if (tmpText == null)
        {
            Text uiText = damageNumberGO.GetComponent<Text>();
            if (uiText != null)
            {
                uiText.text = damageValue.ToString();
            }
            else
            {
                Debug.LogError("�˺�����Ԥ����ȱ��Text�����");
                Destroy(damageNumberGO);
                return null;
            }
        }
        else
        {
            tmpText.text = damageValue.ToString();
        }

        // ��ʼ�����ͽ���Э��
        StartCoroutine(FloatAndFade(damageNumberGO));

        return damageNumberGO;
    }

    // �����ͽ���Ч��
    private IEnumerator FloatAndFade(GameObject go)
    {
        RectTransform rect = go.GetComponent<RectTransform>();
        if (rect == null) yield break; // ��ʼ��ʱ���RectTransform�Ƿ����

        float duration = 1f;
        Vector3 startPos = rect.anchoredPosition;

        Renderer renderer = go.GetComponent<Renderer>();
        if (renderer != null)
        {
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                if (rect == null) break; // ÿ��ѭ��ǰ�ٴμ��
                rect.anchoredPosition = startPos + Vector3.up * 50 * t;
                Color color = renderer.material.color;
                color.a = Mathf.Lerp(1, 0, t / duration);
                renderer.material.color = color;

                yield return null;
            }
        }
        else
        {
            TMP_Text text = go.GetComponent<TMP_Text>();
            if (text != null)
            {
                for (float t = 0; t < duration; t += Time.deltaTime)
                {
                    if (rect == null) break; // ÿ��ѭ��ǰ�ٴμ��
                    rect.anchoredPosition = startPos + Vector3.up * 50 * t;
                    Color color = text.color;
                    color.a = Mathf.Lerp(1, 0, t / duration);
                    text.color = color;

                    yield return null;
                }
            }
        }

        Destroy(go);
    }
}