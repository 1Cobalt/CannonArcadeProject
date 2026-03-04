using UnityEngine;

public class CannonController : MonoBehaviour
{
    [Header("Настройки движения")]
    public float xLimit = 10f;    // Границы экрана (обычно около 2.5 для портретного режима)

    [Header("Настройки стрельбы")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpacing = 0.2f;

    [Header("Характеристики (Загружаются из GameManager)")]
    public int damage = 1;
    public float fireRate = 1f;
    public int weaponLevel = 0;

    private float nextFireTime = 0f;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // 1. БЕРЕМ ГОТОВЫЕ ЦИФРЫ ИЗ МЕНЕДЖЕРА
        // Пушка не считает логарифмы, она просто берет результат.
        if (GameManager.Instance != null)
        {
            damage = GameManager.Instance.GetCalculatedDamage();
            fireRate = GameManager.Instance.GetCalculatedFireRate();
            weaponLevel = GameManager.Instance.GetCalculatedMultiShot();

            Debug.Log($"Stats Loaded: Dmg {damage}, Speed {fireRate}, Multi {weaponLevel}");
        }
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
    }

    // Обработка временных бонусов (PowerUps), которые выпадают во время игры
    public void ApplyBuff(string type)
    {
        switch (type)
        {
            case "Damage":
                damage++; // Просто +1 к урону
                break;

            case "FireRate":
                if (fireRate < 10f) fireRate += 0.5f;
                else fireRate += 0.2f;
                break;

            case "MultiShot":
                weaponLevel++; // +1 дуло
                break;
        }
    }

    // МЕТОД GetCalculatedFireRate ОТСЮДА УДАЛЕН! 
    // ОН ДОЛЖЕН БЫТЬ ТОЛЬКО В GameManager.cs

    void HandleMovement()
    {
        if (Input.GetMouseButton(0)) // Или Input.touchCount > 0 для мобилок
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -mainCamera.transform.position.z; // Важно для корректной работы ScreenToWorldPoint
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePos);

            Vector3 targetPosition = new Vector3(worldPosition.x, transform.position.y, transform.position.z);
            targetPosition.x = Mathf.Clamp(targetPosition.x, -xLimit, xLimit);

            // Можно добавить плавность (Lerp), чтобы пушка не дергалась
            transform.position = Vector3.Lerp(transform.position, targetPosition, 20f * Time.deltaTime);
        }
    }

    void HandleShooting()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            // Расчет следующего выстрела: 1 сек / выстрелов в секунду
            nextFireTime = Time.time + (1f / fireRate);
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null) return;

        // weaponLevel = 0 -> 1 пуля
        // weaponLevel = 1 -> 2 пули
        int bulletCount = 1 + weaponLevel;

        if (bulletCount == 1)
        {
            SpawnBullet(firePoint.position);
        }
        else
        {
            // Математика расстановки пуль в ряд
            float totalWidth = (bulletCount - 1) * bulletSpacing;
            float startX = firePoint.position.x - (totalWidth / 2f);

            for (int i = 0; i < bulletCount; i++)
            {
                float posX = startX + (i * bulletSpacing);
                Vector3 spawnPos = new Vector3(posX, firePoint.position.y, firePoint.position.z);
                SpawnBullet(spawnPos);
            }
        }
    }

    void SpawnBullet(Vector3 position)
    {
        GameObject bulletObj = Instantiate(bulletPrefab, position, Quaternion.identity);
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.damage = damage;

            // Если у нас пули разных цветов для разных уровней
            // bulletScript.SetLevelColor(weaponLevel); 
        }
    }
}