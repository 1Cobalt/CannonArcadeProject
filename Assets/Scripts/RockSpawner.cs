using System.Collections;
using UnityEngine;
using TMPro; // Если вдруг захочешь выводить уровень сложности

public class RockSpawner : MonoBehaviour
{
    [Header("Коллекция врагов")]
    public WaveEnemy[] enemies; // Массив наших типов камней

    [Header("Настройки спавна")]
    public float spawnInterval = 1.0f;
    public float xLimit = 2.2f;

    [Header("Сложность")]
    public int minHealth = 5;
    public int maxHealth = 15;

    [Header("Прогрессия")]
    public float difficultyStepTime = 10f; // Каждые 10 секунд
    public int healthIncrease = 5; // Увеличиваем HP врагов на 5

    private bool isSpawning = true;
    private float timer = 0f;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    void Update()
    {
        // Таймер сложности
        timer += Time.deltaTime;
        if (timer >= difficultyStepTime)
        {
            IncreaseDifficulty();
            timer = 0f; // Сбрасываем таймер
        }
    }

    void IncreaseDifficulty()
    {
        minHealth += healthIncrease;
        maxHealth += healthIncrease;

        // Можно еще и спавнить быстрее, если хочешь жести:
        // spawnInterval = Mathf.Max(0.5f, spawnInterval - 0.05f); 

        Debug.Log($"Сложность повышена! HP врагов: {minHealth}-{maxHealth}");
    }

    IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            SpawnRock();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnRock()
    {
        // 1. Выбираем случайного врага на основе весов
        GameObject selectedPrefab = GetRandomEnemy();

        if (selectedPrefab == null) return;

        float randomX = Random.Range(-xLimit, xLimit);
        Vector3 spawnPos = new Vector3(randomX, transform.position.y, 0);

        // Спавним выбранный префаб
        GameObject newRock = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);

        // ... (дальше твой код настройки HP) ...
    }

    GameObject GetRandomEnemy()
    {
        // Алгоритм "Рулетки" для выбора с учетом веса
        int totalWeight = 0;
        foreach (var enemy in enemies) totalWeight += enemy.spawnChanceWeight;

        int randomValue = Random.Range(0, totalWeight);
        int cursor = 0;

        foreach (var enemy in enemies)
        {
            cursor += enemy.spawnChanceWeight;
            if (randomValue < cursor)
            {
                return enemy.prefab;
            }
        }
        return enemies[0].prefab; // На всякий случай возвращаем первый
    }


}

[System.Serializable]
public class WaveEnemy
{
    public string name; // Просто для удобства в редакторе (например "Red Rock")
    public GameObject prefab;
    [Range(0, 100)] public int spawnChanceWeight; // Вес шанса (чем больше, тем чаще)
}