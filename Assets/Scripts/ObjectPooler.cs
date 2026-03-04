using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler SharedInstance; // Чтобы звать его как "Босс" из любого скрипта
    public GameObject objectToPool; // Префаб пули
    public List<GameObject> pooledObjects; // Сюда сложим наши пули
    
    public int amountToPool; // Сколько пуль создать сразу (например, 50)

    void Awake()
    {
        SharedInstance = this;
    }

    void Start()
    {
        // Инициализируем список и наполняем его выключенными пулями
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = Instantiate(objectToPool);
            obj.SetActive(false); // Сразу выключаем
            pooledObjects.Add(obj);
        }
    }

    // Метод, который будет просить пушка: "Дай мне свободную пулю!"
    public GameObject GetPooledObject()
    {
        // Перебираем список
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            // Если объект НЕ активен в сцене (значит он свободен)
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        // Если все пули заняты, возвращаем null (или можно тут дописать расширение пула)
        return null;
    }
}