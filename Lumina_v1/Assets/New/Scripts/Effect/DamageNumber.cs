using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private float destroyTime = 1f;
    [SerializeField] private Vector3 randomDirectionRange = new Vector3(0.5f, 1f, 0f);

    [SerializeField] private float shakeAmount = 0.1f; // 抖动幅度
    [SerializeField] private float shakeDuration = 0.2f; // 抖动持续时间
    [SerializeField] private float scaleFactor = 1.5f; // 放大系数
    [SerializeField] private float scaleDuration = 0.2f; // 放大持续时间

    private float lifeTimer;
    private Vector3 moveDirection;
    private Vector3 originalPosition;
    private float shakeTimer;
    private float scaleTimer;

    public void Initialize(int damageValue, Vector3 enemyPosition, Color color)
    {
        textMesh.text = damageValue.ToString();
        textMesh.color = color; // 设置跳字颜色

        // 伤害数字显示在敌人位置的右上方
        transform.position = enemyPosition + new Vector3(1f, -0.5f, 0);

        // 随机移动方向
        moveDirection = new Vector3(
            Random.Range(-randomDirectionRange.x, randomDirectionRange.x),
            Random.Range(-randomDirectionRange.y, randomDirectionRange.y),
            Random.Range(-randomDirectionRange.z, randomDirectionRange.z)
        ).normalized;

        lifeTimer = destroyTime;
        originalPosition = transform.position;
        shakeTimer = shakeDuration;
        scaleTimer = scaleDuration;
    }

    private void Update()
    {
        // 移动伤害数字
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // 放大效果
        if (scaleTimer > 0)
        {
            float scale = Mathf.Lerp(1f, scaleFactor, 1 - (scaleTimer / scaleDuration));
            transform.localScale = new Vector3(scale, scale, scale);
            scaleTimer -= Time.deltaTime;
        }

        // 抖动效果
        if (shakeTimer > 0)
        {
            transform.position = originalPosition + Random.insideUnitSphere * shakeAmount;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            transform.position = originalPosition;
        }

        // 渐变效果
        lifeTimer -= Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, 1 - (lifeTimer / destroyTime));
        textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, alpha);

        // 销毁对象
        if (lifeTimer <= 0)
        {
            Destroy(gameObject);
        }
    }
}