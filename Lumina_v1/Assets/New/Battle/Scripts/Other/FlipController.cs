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
            Debug.LogError("SpriteRenderer���δ�ҵ���");
        }
    }

    private void Update()
    {
        // ���Xλ�ô���0����ת���飻���򲻷�ת
        spriteRenderer.flipX = transform.position.x > 0;

    }
}