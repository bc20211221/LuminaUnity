using Lean.Transition.Method;
using UnityEngine;
using UnityEngine.UI;
public class HeadBar : MonoBehaviour
{
    public Slider slider;
    public Text HealthText;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
        HealthText.text = slider.value + "/" + slider.maxValue;
    }
}