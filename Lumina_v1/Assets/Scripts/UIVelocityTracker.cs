using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UIVelocityTracker : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector3 previousPosition;
    public Vector3 currentVelocity;
    private bool isMoving = false;
    
    private float timer = 0.0f;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        previousPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        timer += Time.deltaTime;
        
        Vector3 currentPosition = transform.position;
    
        if (timer < 2) return;
        
        if (!isMoving)
        {
            Vector3 positionDelta = transform.position - previousPosition;
        
            if (positionDelta != Vector3.zero)
            {
                currentVelocity = positionDelta;
            }
    
            previousPosition = currentPosition;
        }
        
        if (isMoving)
        {
            currentVelocity.z = 0;
            
            transform.position += currentVelocity;
        }
    }


    // 开始保持运动
    public void StartMaintainingMovement()
    {
        isMoving = true;
    }

    // 停止保持运动
    public void StopMaintainingMovement()
    {
        isMoving = false;
    }

    // 获取当前速度
    public Vector2 GetCurrentVelocity()
    {
        return currentVelocity;
    }

    // 设置自定义速度
    public void SetVelocity(Vector2 newVelocity)
    {
        currentVelocity = newVelocity;
    }
}