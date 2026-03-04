using UnityEngine;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Header("Тексты UI (Общие)")]
    public TextMeshProUGUI coinsText;

    [Header("Тексты УРОВНЕЙ (Новые!)")]
    // Сюда перетащи текстовые поля, которые стоят над кнопками или рядом
    public TextMeshProUGUI damageLevelText;
    public TextMeshProUGUI fireRateLevelText;
    public TextMeshProUGUI multiShotLevelText;

    [Header("Тексты ЦЕН (На кнопках)")]
    public TextMeshProUGUI damageCostText;
    public TextMeshProUGUI fireRateCostText;
    public TextMeshProUGUI multiShotCostText;

    void Update()
    {
        if (GameManager.Instance == null) return;

        // 1. Монетки
        coinsText.text = "Coins: " + GameManager.Instance.coins;

        // ----------------------------------------------------
        // УРОН (DAMAGE)
        // ----------------------------------------------------
        int dmgLvl = GameManager.Instance.damageLevel;
        int dmgCost = GameManager.Instance.GetUpgradeCost("Damage", dmgLvl);

        // Обновляем отдельный текст уровня (если он назначен)
        if (damageLevelText != null)
            damageLevelText.text = $"Level: {dmgLvl}";

        // Обновляем кнопку с ценой
        if (damageCostText != null)
            damageCostText.text = $"Upgrade\n{dmgCost}";


        // ----------------------------------------------------
        // СКОРОСТЬ (FIRE RATE)
        // ----------------------------------------------------
        int fireLvl = GameManager.Instance.fireRateLevel;
        int fireCost = GameManager.Instance.GetUpgradeCost("FireRate", fireLvl);

        if (fireRateLevelText != null)
            fireRateLevelText.text = $"Level: {fireLvl}";

        if (fireRateCostText != null)
            fireRateCostText.text = $"Upgrade\n{fireCost}";


        // ----------------------------------------------------
        // МУЛЬТИШОТ (MULTISHOT)
        // ----------------------------------------------------
        int multiLvl = GameManager.Instance.multiShotLevel;
        int multiCost = GameManager.Instance.GetUpgradeCost("MultiShot", multiLvl);

        if (multiShotLevelText != null)
        {
            // Для мультишота красивее писать кол-во пуль
            // Ур 0 = 1 пуля, Ур 1 = 2 пули
            multiShotLevelText.text = $"Guns: {multiLvl + 1}";
        }

        if (multiShotCostText != null)
        {
            // Если это первая покупка, можно написать "Unlock"
            if (multiLvl == 0) multiShotCostText.text = $"Unlock\n{multiCost} ";
            else multiShotCostText.text = $"Upgrade\n{multiCost} ";
        }
    }

    // --- МЕТОДЫ КНОПОК ---

    public void OnBuyDamageClick()
    {
        GameManager.Instance.BuyDamageUpgrade();
    }

    public void OnBuyFireRateClick()
    {
        GameManager.Instance.BuyFireRateUpgrade();
    }

    public void OnBuyMultiShotClick()
    {
        GameManager.Instance.BuyMultiShotUpgrade();
    }

    public void OnStartGameClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}