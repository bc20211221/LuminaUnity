using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayMoveEffect : MonoBehaviour
{
    [SerializeField] private float sensitivity = 0.05f;  // 灵敏度
    [SerializeField] private float smoothness = 5f;     // 平滑度
    [SerializeField] private bool invertX = false;      // 是否反转X轴
    [SerializeField] private bool invertY = false;      // 是否反转Y轴

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
        // 获取鼠标在屏幕上的相对位置 (-1 to 1)
        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        // 应用反转设置
        if (invertX) mouseX = -mouseX;
        if (invertY) mouseY = -mouseY;

        // 计算目标位置
        targetPosition = initialPosition + new Vector2(
            mouseX * sensitivity,
            mouseY * sensitivity
        );

        // 平滑移动到目标位置
        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            targetPosition,
            smoothness * Time.deltaTime
        );
    }
}
