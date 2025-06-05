using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))] 
public class FlipController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer组件未找到。");
        }
    }

    private void Update()
    {
        // 如果X位置大于0，翻转精灵；否则不翻转
        spriteRenderer.flipX = transform.position.x > 0;

    }
}