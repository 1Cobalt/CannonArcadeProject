using System.Collections;
using UnityEngine;

public class DropSpawner : MonoBehaviour
{
    [Header("Настройки Спавна")]
    public PathManager pathManager;
    public float spawnInterval = 1f;
    public bool useWaypointRotation = false;

    [Header("Враги (ВАЖНО: Добавь и настрой вес!)")]
    public DropWaveEnemy[] enemies;

    [Header("Баланс Сложности (Гринд)")]
    public int baseHealth = 10;          // Здоровье для Уровня 1
    public int healthPerLevelStep = 25;  // На сколько растет ХП с каждым уровнем

    [System.Serializable]
    public class DropWaveEnemy
    {
        public string name;
        public GameObject prefab;
        [Range(0, 100)] public int spawnChanceWeight;
    }

    [Header("Босс")]
    public GameObject bossPrefab;
    public float timeToBoss = 30f;

    private float timer = 0f;
    private bool bossSpawned = false;

    void Start()
    {
        // 1. ПРОВЕРКА ПУТЕЙ
        if (pathManager == null)
        {
            Debug.LogError("⛔ ОШИБКА: В DropSpawner не назначен Path Manager! Перетащи объект с путями в слот.");
            return;
        }

        if (pathManager.waypoints == null || pathManager.waypoints.Length == 0)
        {
            Debug.LogError("⛔ ОШИБКА: У Path Manager нет точек (waypoints)! Нажми 'Assign Path' или добавь детей в объект.");
            return;
        }

        // 2. ПРОВЕРКА ВРАГОВ
        if (enemies == null || enemies.Length == 0)
        {
            Debug.LogError("⛔ ОШИБКА: Список врагов (Enemies) пуст! Добавь хотя бы одного врага в Инспекторе.");
            return;
        }

        // 3. ПРОВЕРКА ПРЕФАБОВ
        foreach (var e in enemies)
        {
            if (e.prefab == null) Debug.LogError($"⛔ ОШИБКА: У врага '{e.name}' не назначен Prefab!");
            if (e.spawnChanceWeight == 0) Debug.LogWarning($"⚠️ ПРЕДУПРЕЖДЕНИЕ: У врага '{e.name}' шанс появления 0%. Он не появится.");
        }

        Debug.Log("✅ DropSpawner запущен успешно. Ждем спавна...");
        StartCoroutine(SpawnWaves());
    }

    void Update()
    {
        if (!bossSpawned)
        {
            timer += Time.deltaTime;
            if (timer >= timeToBoss)
            {
                bossSpawned = true;
                StopAllCoroutines();
                SpawnBoss();
            }
        }
    }

    IEnumerator SpawnWaves()
    {
        while (!bossSpawned)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(Random.Range(spawnInterval * 0.8f, spawnInterval * 1.2f));
        }
    }

    void SpawnEnemy()
    {
        Transform spawnPoint = GetRandomSpawnPoint();
        GameObject prefab = GetRandomEnemyPrefab();

        if (prefab != null && spawnPoint != null)
        {
            GameObject enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

            // Чистим и вешаем падение (как было раньше)
            var snakeMove = enemy.GetComponent<SnakeMovement>();
            if (snakeMove != null) Destroy(snakeMove);

            DropMovement drop = enemy.AddComponent<DropMovement>();
            if (useWaypointRotation) drop.moveDirection = spawnPoint.up * -1;
            else drop.moveDirection = Vector3.down;

            // --- НОВАЯ МАТЕМАТИКА ЗДОРОВЬЯ ---
            RockHealth hp = enemy.GetComponent<RockHealth>();
            if (hp != null)
            {
                // 1. Узнаем номер уровня из названия сцены (например "Level5" -> 5)
                int levelNum = GetCurrentSceneLevel();

  
                hp.health = baseHealth + (levelNum * healthPerLevelStep) - 25;

                hp.collisionDamage = 1;
            }
        }
    }

    // --- ВСПОМОГАТЕЛЬНЫЙ МЕТОД (Вставь в конец скрипта) ---
    int GetCurrentSceneLevel()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        // Ищем цифры в названии сцены
        System.Text.RegularExpressions.Match match =
            System.Text.RegularExpressions.Regex.Match(sceneName, @"\d+");

        if (match.Success) return int.Parse(match.Value);
        return 1; // Если цифр нет, считаем что это 1 уровень
    }

    void SpawnBoss()
    {
        Debug.Log("💀 ВЫХОД БОССА 💀");
        if (bossPrefab == null)
        {
            Debug.LogError("⛔ ОШИБКА: Boss Prefab не назначен!");
            return;
        }

        // --- ИЗМЕНЕНИЕ ЗДЕСЬ ---
        Transform bossPoint;
        
        // Проверяем, есть ли у нас хотя бы 2 точки
        if (pathManager.waypoints.Length > 1)
        {
            bossPoint = pathManager.waypoints[1]; // Берем ВТОРУЮ точку (индекс 1)
        }
        else
        {
            Debug.LogWarning("⚠️ Внимание: В PathManager меньше 2 точек! Босс спавнится на 1-й.");
            bossPoint = pathManager.waypoints[0]; // Если второй нет, спавним на первой
        }
        // -----------------------

        GameObject boss = Instantiate(bossPrefab, bossPoint.position, Quaternion.identity);

        // Лучше раскомментировать удаление SnakeMovement, если на префабе босса он есть,
        // иначе босс может попытаться ползти как змея, пока выезжает.
        var snakeMove = boss.GetComponent<SnakeMovement>();
        if (snakeMove != null) Destroy(snakeMove);

        boss.AddComponent<BossEntry>();

        RockHealth hp = boss.GetComponent<RockHealth>();
        if (hp != null)
        {
            int levelNum = GetCurrentSceneLevel();

            // Баланс ХП босса
            int mobHP = baseHealth + (levelNum * healthPerLevelStep);
            hp.health = mobHP * 2; 

            hp.collisionDamage = 99999;
        }
    }

    Transform GetRandomSpawnPoint()
    {
        if (pathManager.waypoints.Length == 0) return null;
        int index = Random.Range(0, pathManager.waypoints.Length);
        return pathManager.waypoints[index];
    }

    GameObject GetRandomEnemyPrefab()
    {
        int totalWeight = 0;
        foreach (var e in enemies) totalWeight += e.spawnChanceWeight;

        if (totalWeight == 0)
        {
            Debug.LogError("⛔ ОШИБКА: Сумма шансов (Weights) у врагов равна 0! Поставь хотя бы одному врагу Chance > 0.");
            return null;
        }

        int rnd = Random.Range(0, totalWeight);
        int cursor = 0;

        foreach (var e in enemies)
        {
            cursor += e.spawnChanceWeight;
            if (rnd < cursor) return e.prefab;
        }
        return enemies[0].prefab;
    }
}