using UnityEngine;
using UnityEngine.UI;
using Game.Process;

public class Headquarters : MonoBehaviour
{
    public Text healthText;
    public int currentHealth;
    private HeadBar healthBar;

    public GameObject settlementPanel;
    public GameObject victoryPanel;
    public GameObject defeatPanel;

    void Start()
    {
        healthBar = GetComponentInChildren<HeadBar>();
        healthBar.SetMaxHealth(50);

        BuffAttribute.OnPurityChanged += HandlePurityChanged;
        UpdateHealthText();
        Debug.Log($"当前纯净值：{BuffAttribute.Purity}");

        /*if (settlementPanel != null) settlementPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);*/
    }

    void OnDestory()
    {
        //BuffAttribute.OnPurityChanged -= HandlePurityChanged;
    }

    public void TakeDamage(int amount)
    {
        BuffAttribute.Purity = Mathf.Max(0, Mathf.RoundToInt(BuffAttribute.Purity - amount));
        currentHealth = BuffAttribute.Purity;
        healthBar.SetHealth(BuffAttribute.Purity);
        UpdateHealthText();

        if (BuffAttribute.Purity <= 0)
        {
            // 游戏失败
            GameOver();
        }
    }

    private void HandlePurityChanged(int newPurity)
    {
        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = $"{BuffAttribute.Purity}/"+ 50;
        }
    }

    void GameOver()
    {
        Debug.Log("游戏结束 - 纯净值归零");

        /*if (settlementPanel != null)
        {
            settlementPanel.SetActive(true);
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(true);
        }
        if (defeatPanel != null)
        {
            defeatPanel.SetActive(true);
            victoryPanel.SetActive(false);
        }*/

        // 显示结算结果
        ComboPresenter comboPresenter = FindObjectOfType<ComboPresenter>();
        if (comboPresenter != null)
        {
            comboPresenter.ShowResult();
            Debug.Log("显示结果");
        }

        // 停止游戏时间
        Time.timeScale = 0;
    }
}
