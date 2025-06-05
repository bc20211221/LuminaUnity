using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private float destroyTime = 1f;
    [SerializeField] private Vector3 randomDirectionRange = new Vector3(0.5f, 1f, 0f);

    [SerializeField] private float shakeAmount = 0.1f; // ��������
    [SerializeField] private float shakeDuration = 0.2f; // ��������ʱ��
    [SerializeField] private float scaleFactor = 1.5f; // �Ŵ�ϵ��
    [SerializeField] private float scaleDuration = 0.2f; // �Ŵ����ʱ��

    private float lifeTimer;
    private Vector3 moveDirection;
    private Vector3 originalPosition;
    private float shakeTimer;
    private float scaleTimer;

    public void Initialize(int damageValue, Vector3 enemyPosition, Color color)
    {
        textMesh.text = damageValue.ToString();
        textMesh.color = color; // ����������ɫ

        // �˺�������ʾ�ڵ���λ�õ����Ϸ�
        transform.position = enemyPosition + new Vector3(1f, -0.5f, 0);

        // ����ƶ�����
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
        // �ƶ��˺�����
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // �Ŵ�Ч��
        if (scaleTimer > 0)
        {
            float scale = Mathf.Lerp(1f, scaleFactor, 1 - (scaleTimer / scaleDuration));
            transform.localScale = new Vector3(scale, scale, scale);
            scaleTimer -= Time.deltaTime;
        }

        // ����Ч��
        if (shakeTimer > 0)
        {
            transform.position = originalPosition + Random.insideUnitSphere * shakeAmount;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            transform.position = originalPosition;
        }

        // ����Ч��
        lifeTimer -= Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, 1 - (lifeTimer / destroyTime));
        textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, alpha);

        // ���ٶ���
        if (lifeTimer <= 0)
        {
            Destroy(gameObject);
        }
    }
}