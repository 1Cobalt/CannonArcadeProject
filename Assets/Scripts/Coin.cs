using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 10; // Сколько денег в одной монетке
    public float fallSpeed = 1f;
    public float rotationSpeed = 100f;

    void Update()
    {
        // Падает и крутится для красоты
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime, Space.World);
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        if (transform.position.y < -600f) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Звоним в банк
            GameManager.Instance.AddCoins(value);

            if (other.CompareTag("Player"))
            {
                // Даем валюту
                GameManager.Instance.AddCoins(value);

                // Даем очки (бонус за жадность)
                GameManager.Instance.AddScore(50);

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayCoinSound();
                }

                Destroy(gameObject);
            }

            Destroy(gameObject);
        }
    }
}