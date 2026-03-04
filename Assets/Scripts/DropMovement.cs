using UnityEngine;

public class DropMovement : MonoBehaviour
{
    public float fallSpeed = 1f;
    public float rotationSpeed = 50f;

    // Вектор направления (по умолчанию вниз)
    public Vector3 moveDirection = Vector3.down;

    void Start()
    {
        // Немного случайности скорости
        fallSpeed += Random.Range(-0.2f, 0.4f);
        rotationSpeed = Random.Range(-90f, 90f);

        Destroy(gameObject, 15f); // Удаляем если улетел
    }

    void Update()
    {
        transform.Translate(moveDirection * fallSpeed * Time.deltaTime, Space.World);
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // --- УБИВАЕМ ТЕХ КТО УПАЛ В БЕЗДНУ ---
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }
}