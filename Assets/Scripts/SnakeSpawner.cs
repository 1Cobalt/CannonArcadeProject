using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSpawner : MonoBehaviour
{
    public static SnakeSpawner Instance;

    [Header("Настройки Пути")]
    public PathManager pathManager;

    [Header("Настройки Баланса (СТАРТ)")]
    public float startSpawnInterval = 0.8f; // Чуть медленнее старт
    public int startEnemyHealth = 80;       // РЕСКЕЙЛ: Было 8, стало 80

    [Header("Настройки Прогрессии")]
    public float levelUpTimer = 20f;        // Каждые 20 секунд игра усложняется
    public int healthGrowthPerLevel = 15;   // +15 ХП врагам
    public float spawnRateGrowth = 0.05f;   // -5% к интервалу спавна
    public float minSpawnInterval = 0.3f;   // Предел скорости спавна

    [Header("Физика Змейки (КОНСТАНТЫ)")]
    public float snakeSpeed = 3f;           // НЕ МЕНЯТЬ
    public float retreatSpeed = 8f;         // НЕ МЕНЯТЬ
    public float idealGapDistance = 0.9f;

    [Header("БОСС")]
    public GameObject bossPrefab;
    public int bossHealth = 25000; // 25 тысяч! Нужно ~10 минут стрелять базовой пушкой.

    [Header("Враги")]
    public SnakeWaveEnemy[] enemies;

    [System.Serializable]
    public class SnakeWaveEnemy
    {
        public GameObject prefab;
        [Range(0, 100)] public int spawnChanceWeight;
    }

    // Внутренние переменные
    public List<SnakeMovement> snakeChain = new List<SnakeMovement>();

    // Сюда пишем текущую дистанцию для SnakeMovement
    // Distance = Speed * Interval.
    public float maintainedGap;

    private float currentSpawnInterval;
    private int currentEnemyHealth;
    private float progressionTime;
    private int difficultyLevel = 1;

    void Awake() { Instance = this; }

    void Start()
    {
        snakeChain.Clear();
        currentSpawnInterval = startSpawnInterval;
        currentEnemyHealth = startEnemyHealth;

        // Начальная дистанция сцепки
        maintainedGap = snakeSpeed * currentSpawnInterval;

        if (pathManager != null) StartCoroutine(SpawnRoutine());
    }

    void Update()
    {
        HandleProgression();
    }

    void HandleProgression()
    {
        progressionTime += Time.deltaTime;
        if (progressionTime >= levelUpTimer)
        {
            difficultyLevel++;

            // 1. Враги жирнеют
            currentEnemyHealth += healthGrowthPerLevel;

            // 2. Спавн ускоряется (но не быстрее лимита)
            currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval * (1f - spawnRateGrowth));

            // 3. ВАЖНО: Пересчитываем дистанцию сцепки, так как интервал изменился!
            // Змея станет плотнее
            maintainedGap = snakeSpeed * currentSpawnInterval;

            Debug.Log($"<color=red>УРОВЕНЬ {difficultyLevel}</color>: HP {currentEnemyHealth}, Спавн {currentSpawnInterval:F2}s");

            progressionTime = 0f;
        }
    }

    IEnumerator SpawnRoutine()
    {
        // Босс
        if (bossPrefab != null) SpawnObject(bossPrefab, true);
        yield return new WaitForSeconds(currentSpawnInterval);

        // Волна
        while (true)
        {
            GameObject prefab = GetRandomEnemy();
            if (prefab != null) SpawnObject(prefab, false);
            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }

    void SpawnObject(GameObject prefab, bool isBoss)
    {
        Transform startPoint = pathManager.waypoints[0];
        GameObject obj = Instantiate(prefab, startPoint.position, Quaternion.identity);

        RockHealth hp = obj.GetComponent<RockHealth>();
        if (hp != null)
        {
            if (isBoss)
            {
                obj.transform.localScale = Vector3.one * 2.5f;
                hp.health = bossHealth;
                hp.scoreValue = 10000;
            }
            else
            {
                // Обычным врагам даем ТЕКУЩЕЕ увеличенное здоровье
                hp.health = currentEnemyHealth;
            }
        }

        SetupMovement(obj);
    }

    void SetupMovement(GameObject obj)
    {
        SimpleMove old = obj.GetComponent<SimpleMove>();
        if (old != null) Destroy(old);

        SnakeMovement snakeMove = obj.AddComponent<SnakeMovement>();
        snakeChain.Add(snakeMove);
        snakeMove.Initialize(pathManager.waypoints);
    }

    // --- ZUMA LOGIC ---
    public void OnRockDied(SnakeMovement deadRock)
    {
        int index = snakeChain.IndexOf(deadRock);
        if (index > 0 && index < snakeChain.Count)
        {
            SnakeMovement rockAhead = snakeChain[index - 1];
            if (rockAhead != null) rockAhead.isBroken = true;
        }
        snakeChain.Remove(deadRock);
    }

    public void RemoveFromChain(SnakeMovement rock)
    {
        if (snakeChain.Contains(rock)) snakeChain.Remove(rock);
    }

    GameObject GetRandomEnemy()
    {
        if (enemies.Length == 0) return null;
        int w = 0; foreach (var e in enemies) w += e.spawnChanceWeight;
        int r = Random.Range(0, w);
        int c = 0;
        foreach (var e in enemies) { c += e.spawnChanceWeight; if (r < c) return e.prefab; }
        return enemies[0].prefab;
    }
}