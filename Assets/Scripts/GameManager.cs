using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Состояние Игры")]
    public int coins;
    public int score;
    public int highScore;

    [Header("Текущие Уровни (Save Data)")]
    public int damageLevel = 1;
    public int fireRateLevel = 1;
    public int multiShotLevel = 0;

    // ---------------------------------------------------------
    // ВОТ ТВОИ РЫЧАГИ УПРАВЛЕНИЯ БАЛАНСОМ
    // ---------------------------------------------------------

    [Header("НАСТРОЙКИ: Характеристики Пушки")]
    [Tooltip("Урон на 1 уровне")]
    public int baseDamage = 10;
    [Tooltip("Сколько урона добавляет каждый апгрейд")]
    public int damagePerLevel = 2;

    [Tooltip("Выстрелов в секунду на 1 уровне")]
    public float baseFireRate = 4.0f;
    [Tooltip("Сколько скорости добавляет каждый апгрейд")]
    public float fireRatePerLevel = 0.3f;

    [Header("НАСТРОЙКИ: Цены в Магазине")]
    [Tooltip("Цена Урона = Уровень * ЭтоЧисло. (Пример: 100 -> 1 ур = 100, 2 ур = 200)")]
    public int damagePriceStep = 100;

    [Tooltip("Цена Скорости = Уровень * ЭтоЧисло")]
    public int fireRatePriceStep = 150;

    [Tooltip("Цена Первого Мультишота (0 -> 1)")]
    public int multiShotBasePrice = 1000;
    [Tooltip("Цена следующих Мультишотов (Уровень * ЭтоЧисло)")]
    public int multiShotPriceStep = 2500;

    void Awake()
    {
        score = 0;
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
        }
        else Destroy(gameObject);
    }

    // --- ЭКОНОМИКА ---

    public int GetUpgradeCost(string type, int currentLevel)
    {
        switch (type)
        {
            case "Damage":
                // Формула: Уровень * ШагЦены
                // Lvl 1: 1 * 100 = 100
                // Lvl 5: 5 * 100 = 500
                return currentLevel * damagePriceStep;

            case "FireRate":
                return currentLevel * fireRatePriceStep;

            case "MultiShot":
                // Первый уровень особый (фиксированная цена)
                if (currentLevel == 0) return multiShotBasePrice;
                // Дальше растет прогрессивно
                return currentLevel * multiShotPriceStep;

            default: return 999999;
        }
    }

    // --- ХАРАКТЕРИСТИКИ ---

    public int GetCalculatedDamage()
    {
        // База + (Уровень-1 * Прирост)
        // Если База 10, Прирост 2:
        // Ур 1: 10 + 0 = 10
        // Ур 2: 10 + 2 = 12
        return baseDamage + ((damageLevel - 1) * damagePerLevel);
    }

    public float GetCalculatedFireRate()
    {
        if (fireRateLevel <= 1) return baseFireRate;

        return baseFireRate + (Mathf.Log(fireRateLevel) * fireRatePerLevel);
    }

    public int GetCalculatedMultiShot()
    {
        return multiShotLevel;
    }

    // --- ПОКУПКИ (Без изменений, просто вызывают методы выше) ---

    public void BuyDamageUpgrade()
    {
        int cost = GetUpgradeCost("Damage", damageLevel);
        if (TrySpendCoins(cost)) { damageLevel++; SaveProgress(); Debug.Log("Damage Up!"); }
    }

    public void BuyFireRateUpgrade()
    {
        int cost = GetUpgradeCost("FireRate", fireRateLevel);
        if (TrySpendCoins(cost)) { fireRateLevel++; SaveProgress(); }
    }

    public void BuyMultiShotUpgrade()
    {
        int cost = GetUpgradeCost("MultiShot", multiShotLevel);
        if (TrySpendCoins(cost)) { multiShotLevel++; SaveProgress(); }
    }

    public bool TrySpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            SaveProgress();
            return true;
        }
        return false;
    }

    public void AddCoins(int amount) { coins += amount; SaveProgress(); }

    public void AddScore(int amount)
    {
        score += amount;
        if (score > highScore) { highScore = score; PlayerPrefs.SetInt("HighScore", highScore); }
    }

    // --- СОХРАНЕНИЕ ---
    void SaveProgress()
    {
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.SetInt("DmgLvl", damageLevel);
        PlayerPrefs.SetInt("FireLvl", fireRateLevel);
        PlayerPrefs.SetInt("MultiLvl", multiShotLevel);
        PlayerPrefs.Save();
    }

    void LoadProgress()
    {
        coins = PlayerPrefs.GetInt("Coins", 0);
        damageLevel = PlayerPrefs.GetInt("DmgLvl", 1);
        fireRateLevel = PlayerPrefs.GetInt("FireLvl", 1);
        multiShotLevel = PlayerPrefs.GetInt("MultiLvl", 0);
    }

    // Для вызова из игры
    public void WinLevel() { if (Camera.main.GetComponent<MainMenu>()) Camera.main.GetComponent<MainMenu>().FinishGame(true); }
    public void LoseLevel() { if (Camera.main.GetComponent<MainMenu>()) Camera.main.GetComponent<MainMenu>().FinishGame(false); }
}