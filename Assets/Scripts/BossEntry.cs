using UnityEngine;

public class BossEntry : MonoBehaviour
{
    [Header("Настройки Появления")]
    public float enterSpeed = 3f;      // Быстро выезжает в начале
    public float stopYPosition = 3.5f; // Где начинает бой (верхняя часть)

    [Header("Настройки Боя")]
    public float patrolSpeed = 3f;     // Скорость влево-вправо
    public float creepSpeed = 0.4f;    // <--- НОВОЕ: Скорость медленного сползания вниз
    public float screenLimitX = 10.0f;    // Границы экрана

    private bool isInPosition = false;
    private bool movingRight = true;

    void Start()
    {
        // Выравниваем босса, чтобы не летел боком
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        if (!isInPosition)
        {
            // ФАЗА 1: Быстрый выход на позицию
            transform.Translate(Vector3.down * enterSpeed * Time.deltaTime, Space.World);

            // Если доехали до верха экрана
            if (transform.position.y <= stopYPosition)
            {
                isInPosition = true;
            }
        }
        else
        {
            // ФАЗА 2: Бой (Патруль + Давление)

            // 1. Движение Влево-Вправо
            float moveX = patrolSpeed * Time.deltaTime * (movingRight ? 1 : -1);

            // 2. Движение Вниз (Медленное давление)
            float moveY = -creepSpeed * Time.deltaTime; // Минус, потому что вниз

            // Применяем оба движения сразу
            transform.Translate(new Vector3(moveX, moveY, 0), Space.World);

            // Логика отталкивания от стен
            if (transform.position.x > screenLimitX) movingRight = false;
            if (transform.position.x < -screenLimitX) movingRight = true;
        }
    }
}