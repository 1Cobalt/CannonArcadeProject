using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 1;

    // Защита от двойного урона (дробовик)
    private bool hasHit = false;

    // Граница экрана сверху, после которой пуля удаляется
    // (Подбери под свой телефон, обычно 6-7 хватает, 10 с запасом)
    private float topBound = 10f;

    void Update()
    {
        // Твой код полета (Translate)
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        // --- ДОБАВЬ ЭТО ---
        // Если пуля улетела слишком высоко (например, Y > 6)
        if (transform.position.y > 6f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Если уже попали — игнорируем всё остальное
        if (hasHit) return;

        // Попадание во врага
        if (other.CompareTag("Enemy"))
        {
            hasHit = true;

            RockHealth enemy = other.GetComponent<RockHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject); // Уничтожаем пулю
        }
    }
}