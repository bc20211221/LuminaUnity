using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Text buttonText;
    public Outline textOutline;
    public Image leftCursorImage;
    public Image rightCursorImage;
    public AudioSource hoverAudioSource; // 悬停音效
    public AudioSource clickAudioSource; // 点击音效

    private Color originalTextColor;
    private bool isOutlineEnabled;

    void Start()
    {
        if (buttonText != null)
        {
            originalTextColor = buttonText.color;
            originalTextColor.a = 0.8f;
            buttonText.color = originalTextColor;
        }

        if (textOutline != null)
        {
            isOutlineEnabled = textOutline.enabled;
            textOutline.enabled = false;
        }

        if (leftCursorImage != null)
        {
            leftCursorImage.gameObject.SetActive(false);
        }

        if (rightCursorImage != null)
        {
            rightCursorImage.gameObject.SetActive(false);
            // 确保点击音效的 AudioSource 配置正确
            if (clickAudioSource != null)
            {
                clickAudioSource.playOnAwake = false;
            }
        }

        if (hoverAudioSource != null)
        {
            hoverAudioSource.playOnAwake = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            Color newColor = originalTextColor;
            newColor.a = 1f;
            buttonText.color = newColor;
        }

        if (textOutline != null)
        {
            textOutline.enabled = true;
        }

        if (leftCursorImage != null)
        {
            leftCursorImage.gameObject.SetActive(true);
        }

        if (rightCursorImage != null)
        {
            rightCursorImage.gameObject.SetActive(true);
        }

        // 播放悬停音效
        if (hoverAudioSource != null && hoverAudioSource.clip != null)
        {
            hoverAudioSource.Play();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            buttonText.color = originalTextColor;
        }

        if (textOutline != null)
        {
            textOutline.enabled = isOutlineEnabled;
        }

        if (leftCursorImage != null)
        {
            leftCursorImage.gameObject.SetActive(false);
        }

        if (rightCursorImage != null)
        {
            rightCursorImage.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 播放点击音效
        if (clickAudioSource != null && clickAudioSource.clip != null)
        {
            clickAudioSource.Play();
        }
    }
}