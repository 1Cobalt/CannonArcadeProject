using UnityEngine;
using TMPro;

public class RockHealth : MonoBehaviour
{
    [Header("Экономика")]
    public GameObject coinPrefab;
    [Range(0f, 1f)] public float coinDropChance = 0.3f; // 30% шанс
    public int minCoins = 1;
    public int maxCoins = 3;

    [Header("Характеристики")]
    public int health = 20;
    public int scoreValue = 10;
    public int collisionDamage = 1;

    [Header("Ссылки")]
    public TextMeshPro textHP;
    public GameObject deathEffect;

    [Header("Лут (Дроп)")]
    public GameObject dropItem; // Сюда кладем префаб баффа (Red/Blue/Gold)
    [Range(0f, 1f)] public float dropChance = 1f; // Шанс выпадения (1 = 100%)

    void Start()
    {
        UpdateText();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        UpdateText();

        if (health <= 0)
        {
            Die();
        }
    }

    void UpdateText()
    {
        if (textHP != null)
        {
            textHP.text = health.ToString();
        }
    }

    void Die()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayRockBreak();
        }
        // 2. ВЫПАДЕНИЕ ЛУТА (Вот то, что пропало!)
        if (dropItem != null && Random.value <= dropChance)
        {
            Instantiate(dropItem, transform.position, Quaternion.identity);
        }

        if (coinPrefab != null && Random.value <= coinDropChance)
        {
            int amount = Random.Range(minCoins, maxCoins);
            for (int i = 0; i < amount; i++)
            {
                // Спавним монетки с небольшим разбросом
                Vector3 randomOffset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0);
                Instantiate(coinPrefab, transform.position + randomOffset, Quaternion.identity);
            }
        }
        // 3. Логика Змейки (если это уровень со змейкой)
        SnakeMovement snakeMove = GetComponent<SnakeMovement>();
        if (snakeMove != null)
        {
            snakeMove.OnDestroyByPlayer(); // <--- ВОТ ЭТО ВАЖНО
        }
     
        if (scoreValue >= 1000)
        {


        
            GameManager.Instance.WinLevel();
    
        }

        // 4. Уничтожение
        Destroy(gameObject);
    }

    // В RockHealth.cs

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // ИСПОЛЬЗУЕМ НАШУ ПЕРЕМЕННУЮ
                playerHealth.TakeDamage(collisionDamage);
            }

            // Логика разрыва цепи (Зума)
            SnakeMovement snakeMove = GetComponent<SnakeMovement>();
            if (snakeMove != null)
            {
                snakeMove.OnDestroyByPlayer();
            }

            // Камень уничтожается при ударе
            Destroy(gameObject);
        }
    }
}