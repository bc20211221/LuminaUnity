using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayMoveEffect : MonoBehaviour
{
    [SerializeField] private float sensitivity = 0.05f;  // ������
    [SerializeField] private float smoothness = 5f;     // ƽ����
    [SerializeField] private bool invertX = false;      // �Ƿ�תX��
    [SerializeField] private bool invertY = false;      // �Ƿ�תY��

    private RectTransform rectTransform;
    private Vector2 initialPosition;
    private Vector2 targetPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        // ��ȡ�������Ļ�ϵ����λ�� (-1 to 1)
        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        // Ӧ�÷�ת����
        if (invertX) mouseX = -mouseX;
        if (invertY) mouseY = -mouseY;

        // ����Ŀ��λ��
        targetPosition = initialPosition + new Vector2(
            mouseX * sensitivity,
            mouseY * sensitivity
        );

        // ƽ���ƶ���Ŀ��λ��
        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            targetPosition,
            smoothness * Time.deltaTime
        );
    }
}
