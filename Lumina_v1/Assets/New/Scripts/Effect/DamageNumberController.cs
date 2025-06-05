using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class DamageNumberController : MonoBehaviour
{
    public static DamageNumberController Instance; // 确保单例实例
    public GameObject damageNumberPrefab; // 使用GameObject类型，可能是UI的TextMeshPro
    public Transform canvasTransform; // 确保获取到Canvas的Transform

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // 可能需要一些资源释放的逻辑，这里简单销毁
            Destroy(gameObject);
        }
    }

    // 生成伤害数字的方法
    public GameObject SpawnDamageNumber(Vector3 worldPosition, int damageValue)
    {
        Debug.Log($"生成伤害数字位置：{worldPosition}"); // 调试输出
        if (damageNumberPrefab == null)
        {
            Debug.LogError("DamageNumberPrefab未赋值！");
            return null;
        }

        // 实例化预制体到Canvas下
        GameObject damageNumberGO = Instantiate(
            damageNumberPrefab,
            worldPosition,
            Quaternion.identity,
            canvasTransform
        );

        // 获取文本组件，可能是TextMeshPro或Unity UI Text
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
                Debug.LogError("伤害数字预制体缺少Text组件！");
                Destroy(damageNumberGO);
                return null;
            }
        }
        else
        {
            tmpText.text = damageValue.ToString();
        }

        // 开始浮动和渐隐协程
        StartCoroutine(FloatAndFade(damageNumberGO));

        return damageNumberGO;
    }

    // 浮动和渐隐效果
    private IEnumerator FloatAndFade(GameObject go)
    {
        RectTransform rect = go.GetComponent<RectTransform>();
        if (rect == null) yield break; // 初始化时检查RectTransform是否存在

        float duration = 1f;
        Vector3 startPos = rect.anchoredPosition;

        Renderer renderer = go.GetComponent<Renderer>();
        if (renderer != null)
        {
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                if (rect == null) break; // 每次循环前再次检查
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
                    if (rect == null) break; // 每次循环前再次检查
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