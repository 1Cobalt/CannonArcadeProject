using UnityEngine;

// Оставляем Enum, чтобы в Инспекторе был удобный список!
public enum PowerUpType { FireRate, Damage, MultiShot }

public class PowerUp : MonoBehaviour
{
    public PowerUpType type;
    public float fallSpeed = 5f;

    void Update()
    {
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

        if (transform.position.y < -600f) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CannonController cannon = other.GetComponent<CannonController>();
            if (cannon != null)
            {
                cannon.ApplyBuff(type.ToString());
            }
            if (GameManager.Instance != null)
                GameManager.Instance.AddScore(100);

            Destroy(gameObject);
        }
    }
}